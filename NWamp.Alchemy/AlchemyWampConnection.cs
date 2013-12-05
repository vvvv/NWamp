namespace NWamp.Transport
{
    using Alchemy.Classes;
    using NWamp.Mapping;
    using NWamp.Transport;

    /// <summary>
    /// WAMP connection implementation using Alchemy websockets.
    /// </summary>
    public class AlchemyWampConnection : IWampConnection
    {
        /// <summary>
        /// Gets or sets WAMP session identifier for current connection.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets Alchemy websockets user context associated with current connection.
        /// </summary>
        public UserContext UserContext { get; private set; }

        /// <summary>
        /// Gets list of CURIE-URI prefixes specific for current connection.
        /// </summary>
        public PrefixMap Prefixes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampConnection"/> class using provided 
        /// Alchemy <see cref="UserContext"/> connection object.
        /// </summary>
        /// <param name="context">Context used for accessing related web socket connection.</param>
        public AlchemyWampConnection(UserContext context)
        {
            this.Prefixes = new PrefixMap();
            this.UserContext = context;
        }

        /// <summary>
        /// Sends a JSON-stringified object through web socket connection.
        /// </summary>
        /// <param name="json">String representing object in JSON notation.</param>
        public void SendMessage(string json)
        {
            this.UserContext.Send(json);
        }
    }
}
