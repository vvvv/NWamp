namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments class used before new connection will subscribe to provided topic.
    /// </summary>
    public class ConnectionSubscribingEventArgs : PubSubEventArgs
    {
        public bool Cancel { get; set; }

        public ConnectionSubscribingEventArgs(string topicId, string sessionId)
            : base(topicId, sessionId)
        {
        }
    }

    public delegate void ConnectionSubscribingEventHandler(object sender, ConnectionSubscribingEventArgs e);
}
