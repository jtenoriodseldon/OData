using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataAPI.DTOs;
using ODataAPI.Models;
using ODataAPI.Results;
using ODataAPI.Settings;
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ODataAPI.Controllers
{
    /// <summary>
    /// Controller to handle customer-related operations.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class CustomersController : ODataController
    {
        #region Members
        private readonly ILogger<CustomersController> _logger;
        private readonly AdventureWorksLT2019Context _dbContext;
        private readonly ODataSettings _odataSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="oDataSettings">The OData settings.</param>
        public CustomersController(ILogger<CustomersController> logger, AdventureWorksLT2019Context dbContext, ODataSettings oDataSettings)
        {
            _logger = logger;
            _dbContext = dbContext;
            _odataSettings = oDataSettings;
        }

        #endregion

        #region API

        /// <summary>
        /// Retrieves all customers with OData query options.
        /// </summary>
        /// <param name="options">The OData query options.</param>
        /// <returns>A collection of customers or a count of customers based on the query options.</returns>
        [HttpGet]
        [Route("api/customers")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ODataResultEnvelopeCollection<Customer>), Status200OK)]
        [ProducesResponseType(typeof(ODataEnvelope), Status404NotFound)]
        [ProducesResponseType(typeof(ODataCustomError), Status400BadRequest)]
        public IActionResult RetrieveAll([SwaggerIgnore] ODataQueryOptions<Customer> options)
        {
            return ODataCustomResult.ObtainResult(options, _dbContext.Customers.AsQueryable(), _odataSettings);
        }

        #endregion
    }
}
