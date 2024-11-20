namespace ODataAPI.Settings
{
    /// <summary>
    /// Represents the settings for OData configuration.
    /// </summary>
    public class ODataSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the host name for the OData service.
        /// </summary>
        public required string HostName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS is used.
        /// </summary>
        public bool IsHttps { get; set; }

        /// <summary>
        /// Gets or sets the maximum page size for OData queries.
        /// </summary>
        public int MaxPageSize { get; set; } = 2500;

        /// <summary>
        /// Gets or sets the default page size for OData queries.
        /// </summary>
        public int DefaultPageSize { get; set; } = 250;

        #endregion
    }
}
