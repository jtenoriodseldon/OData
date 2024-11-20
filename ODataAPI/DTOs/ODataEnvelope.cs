namespace ODataAPI.DTOs
{
    /// <summary>
    /// Represents an envelope for OData responses.
    /// </summary>
    public class ODataEnvelope
    {
        #region Properties

        /// <summary>
        /// Gets or sets the total count of items.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the size of each page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the URL for the next page.
        /// </summary>
        public string? NextUrl { get; set; }

        #endregion
    }
}
