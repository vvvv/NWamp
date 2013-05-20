namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments used after existing topic has been removed.
    /// </summary>
    public class TopicRemovedEventArgs : PubSubEventArgs
    {
        public TopicRemovedEventArgs(string topicId)
            : base(topicId, null)
        {
        }
    }

    public delegate void TopicRemovedEventHandler(object sender, TopicRemovedEventArgs e);
}
