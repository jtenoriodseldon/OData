namespace ODataAPI.DTOs
{
    /// <summary>
    /// Represents a projection of OData result envelope.
    /// </summary>
    public class ODataResultEnvelopeProjection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the queryable results.
        /// </summary>
        public IQueryable? Results { get; set; }

        /// <summary>
        /// Gets or sets the OData envelope.
        /// </summary>
        public ODataEnvelope? Envelope { get; set; }

        #endregion
    }
}
