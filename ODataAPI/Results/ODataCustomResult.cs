using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData;
using Newtonsoft.Json;
using ODataAPI.DTOs;
using ODataAPI.Settings;
using System.Collections;

namespace ODataAPI.Results
{
    /// <summary>
    /// OData Custom Result
    /// </summary>
    public static class ODataCustomResult
    {
        #region Methods

        /// <summary>
        /// Obtains the OData result based on the provided query options, queryable data, and OData settings.
        /// </summary>
        /// <typeparam name="T">The type of the data in the queryable.</typeparam>
        /// <param name="options">The OData query options.</param>
        /// <param name="queryable">The queryable data source.</param>
        /// <param name="odataSettings">The OData settings.</param>
        /// <returns>An IActionResult containing the OData result.</returns>
        public static IActionResult ObtainResult<T>(ODataQueryOptions<T> options, IQueryable queryable, ODataSettings odataSettings)
        {
            if (queryable == null)
            {
                return new NotFoundObjectResult(
                    new ODataEnvelope
                    {
                        Count = 0,
                        PageSize = 0,
                        TotalPages = 0,
                        CurrentPage = 0,
                        NextUrl = string.Empty
                    });
            }

            var pageSize = odataSettings.DefaultPageSize;
            var maximumPageSize = odataSettings.MaxPageSize;

            // Add the count context and use the alternate options, have to do this to inject into the ODataQueryOptions pipeline
            if (options.Count == null)
                options.Request.QueryString = options.Request.QueryString.Add("$count", "true");

            // Because the get next URL takes current context, we need to change it to the context of the frontend
            options.Request.Host = new HostString(odataSettings.HostName);
            options.Request.IsHttps = odataSettings.IsHttps;

            var alternateOptions = new ODataQueryOptions<T>(options.Context, options.Request);

            long? count;

            // Perform count with or without filter, this will give us the correct count and page size
            try
            {
                if (alternateOptions.Filter != null)
                    count = alternateOptions.Count.GetEntityCount(alternateOptions.Filter.ApplyTo(queryable, new ODataQuerySettings()));
                else
                    count = alternateOptions.Count.GetEntityCount(queryable);
            }
            catch (Exception)
            {
                count = null;
            }

            try
            {
                // Obtain results, if top querystring exists, then use that one instead of the default, also make sure the top does not go over the limit
                IQueryable? results = null;
                if (alternateOptions.Top != null)
                {
                    if (alternateOptions.Top.Value > maximumPageSize || options.Top.Value < 1)
                        return new BadRequestObjectResult($"The page size cannot exceed {maximumPageSize} or be less than 1");

                    pageSize = alternateOptions.Top.Value;
                    results = alternateOptions.ApplyTo(queryable, new ODataQuerySettings { PageSize = pageSize });
                }
                else
                    results = alternateOptions.ApplyTo(queryable, new ODataQuerySettings { PageSize = pageSize });

                // BUG with OData, have to cast to IEnumerable, since casting to specific DTO does not work as the transmutation is from SelectMany to DTO
                // IEnumerable will work as it does not care about the type, performance is impacted, but not that much
                if (count == null)
                {
                    try
                    {
                        count = results.Cast<T>().Count();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            count = results.Cast<IEnumerable>().Count();
                        }
                        catch (Exception)
                        {
                            // Edge case, serialize to dynamic, as it does not matter the type at this moment, just the count
                            // This is slow as it will serialize twice
                            var jsonResult = JsonConvert.SerializeObject(results);
                            if (jsonResult != null)
                            {
                                var dynamicResult = JsonConvert.DeserializeObject(jsonResult) as dynamic;
                                if (dynamicResult != null)
                                    count = dynamicResult.Count;
                            }
                        }
                    }
                }

                // Calculate envelope

                var skip = alternateOptions.Skip != null ? alternateOptions.Skip.Value : 0;
                var currentPage = (skip + pageSize) / pageSize;
                var totalPages = (int)Math.Ceiling((decimal)(count ?? 0) / pageSize);
                string? nextUrl = null;

                if (currentPage < totalPages)
                {
                    try
                    {
                        nextUrl = alternateOptions.Request.GetNextPageLink(pageSize, null, null)?.ToString().Replace("$top=0", $"$top={pageSize}");
                        if (nextUrl == null)
                        {
                            // Calculate old fashion
                            var http = odataSettings.IsHttps ? "https://" : "http://";
                            var queryString = alternateOptions.Request.Query;
                            var finalQueryString = "?";
                            foreach (var query in queryString)
                            {
                                if (!query.Key.Contains("top") && !query.Key.Contains("skip"))
                                    finalQueryString += $"{query.Key}={query.Value}&";
                            }
                            finalQueryString += $"$top={pageSize}&$skip={pageSize * currentPage}";

                            nextUrl = $"{http}{odataSettings.HostName}{alternateOptions.Request.Path}{finalQueryString}";
                        }
                    }
                    catch (Exception)
                    {
                        nextUrl = null;
                    }
                }

                try
                {
                    if (count <= 0)
                        return new NotFoundObjectResult(
                            new ODataEnvelope
                            {
                                Count = 0,
                                CurrentPage = 0,
                                NextUrl = string.Empty,
                                PageSize = 0,
                                TotalPages = 0
                            });

                    return new OkObjectResult(
                        new ODataResultEnvelopeCollection<T>
                        {
                            Envelope = new ODataEnvelope
                            {
                                Count = (int)(count.HasValue ? count.Value : 0),
                                PageSize = pageSize,
                                TotalPages = totalPages,
                                CurrentPage = currentPage,
                                NextUrl = nextUrl
                            },
                            Results = count >= 0 ? [.. results.Cast<T>()] : null
                        });
                }
                catch (InvalidCastException)
                {
                    return new OkObjectResult(
                        new ODataResultEnvelopeProjection
                        {
                            Envelope = new ODataEnvelope
                            {
                                Count = (int)(count.HasValue ? count.Value : 0),
                                PageSize = pageSize,
                                TotalPages = totalPages,
                                CurrentPage = currentPage,
                                NextUrl = nextUrl
                            },
                            Results = count >= 0 ? results : null
                        });
                }
            }
            catch (ODataException ex)
            {
                return new BadRequestObjectResult(
                    new ODataCustomError
                    {
                        ErrorMessage = ex.Message
                    });
            }
        }

        #endregion Methods
    }
}