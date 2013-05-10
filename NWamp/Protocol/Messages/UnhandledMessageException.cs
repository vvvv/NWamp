namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Exception thrown, when message type is not supported or cannot be handled.
    /// </summary>
    public class UnhandledMessageException : WampException
    {
        public IWampMessage WampMessage { get; set; }

        public UnhandledMessageException(string msg) : base(msg)
        {
        }

        public UnhandledMessageException(string msg, IWampMessage message) : base(msg)
        {
            this.WampMessage = message;
        }
    }
}