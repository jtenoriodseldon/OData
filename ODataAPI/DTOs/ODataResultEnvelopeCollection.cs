namespace ODataAPI.DTOs
{
    /// <summary>
    /// Represents a collection of results with an associated OData envelope.
    /// </summary>
    /// <typeparam name="T">The type of the results contained in the collection.</typeparam>
    public class ODataResultEnvelopeCollection<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of results.
        /// </summary>
        public List<T>? Results { get; set; }

        /// <summary>
        /// Gets or sets the OData envelope associated with the results.
        /// </summary>
        public ODataEnvelope? Envelope { get; set; }

        #endregion
    }
}
