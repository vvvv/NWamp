namespace NWamp.Transport
{
    using NWamp.Mapping;

    /// <summary>
    /// Interface representing single WAMP client-server connection.
    /// </summary>
    public interface IWampConnection
    {
        /// <summary>
        /// Gets or sets WAMP session identifier for current connection.
        /// </summary>
        string SessionId { get; set; }

        /// <summary>
        /// Gets or sets mapping dictionary of CURIE-URI prefixes. 
        /// Those prefixes are specific for each connection.
        /// </summary>
        PrefixMap Prefixes { get; }

        /// <summary>
        /// Sends new JSON string through web socket connection.
        /// </summary>
        void SendMessage(string json);
    }
}