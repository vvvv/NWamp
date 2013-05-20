namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments created after existing connection has unsubcribed.
    /// </summary>
    public class ConnectionUnsubscribedEventArgs : PubSubEventArgs
    {

        public ConnectionUnsubscribedEventArgs(string topicId, string sessionId)
            :base(topicId, sessionId)
        {
        }
    }

    public delegate void ConnectionUnsubscribedEventHandler(object sender, ConnectionUnsubscribedEventArgs e);
}
