namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments class used after new connection has successfully subscribed to provided topic.
    /// </summary>
    public class ConnectionSubscribedEventArgs : PubSubEventArgs
    {
        public ConnectionSubscribedEventArgs(string topicId, string sessionId)
            : base(topicId, sessionId)
        {
        }
    }

    public delegate void ConnectionSubscribedEventHandler(object sender, ConnectionSubscribedEventArgs e);
}
