using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ODataAPI.Filter
{
    /// <summary>
    /// A filter that adds OData query parameters to the OpenAPI documentation.
    /// </summary>
    public class ODATAParamsFilter : IOperationFilter
    {
        #region Methods

        /// <summary>
        /// A list of OData query parameters to be added to the OpenAPI documentation.
        /// </summary>
        private static readonly List<OpenApiParameter> ODataOpenAPIParameters = (new List<(string Name, string Description)>()
            {
                ( "$top", "The max number of records to return."),
                ( "$skip", "The number of records to skip."),
                ( "$filter", "A function that must evaluate to true for a record to be returned."),
                ( "$select", "Specifies a subset of properties to return."),
                ( "$orderby", "Determines which values are used to order a collection of records."),
                ( "$expand", "Use to add related query data.")
            }).Select(pair => new OpenApiParameter
            {
                Name = pair.Name,
                Required = false,
                Schema = new OpenApiSchema { Type = "String" },
                In = ParameterLocation.Query,
                Description = pair.Description,
                AllowReserved = true
            }).ToList();

        /// <summary>
        /// Applies the OData query parameters to the given OpenAPI operation.
        /// </summary>
        /// <param name="operation">The OpenAPI operation to modify.</param>
        /// <param name="context">The context for the operation filter.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            try
            {
                if (context.ApiDescription.ParameterDescriptions[0].ModelMetadata.ModelType.Name.Contains("ODataQueryOptions", StringComparison.InvariantCultureIgnoreCase))
                {
                    operation.Parameters ??= [];
                    foreach (var item in ODataOpenAPIParameters)
                        operation.Parameters.Add(item);
                }
            }
            catch (Exception)
            {
                //ignore and continue
            }
        }

        #endregion Methods
    }
}