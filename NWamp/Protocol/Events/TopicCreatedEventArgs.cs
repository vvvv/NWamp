namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments class created after new topic has been created.
    /// </summary>
    public class TopicCreatedEventArgs : PubSubEventArgs
    {
        public TopicCreatedEventArgs(string topicId)
            : base(topicId, null)
        {
        }
    }

    public delegate void TopicCreatedEventHandler(object sender, TopicCreatedEventArgs e);
}
