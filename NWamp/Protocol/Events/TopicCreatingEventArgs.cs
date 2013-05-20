namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments class created before new topic has been created.
    /// </summary>
    public class TopicCreatingEventArgs : PubSubEventArgs
    {
        public bool Cancel { get; set; }

        public TopicCreatingEventArgs(string topicId)
            : base(topicId, null)
        {
        }
    }

    public delegate void TopicCreatingEventHandler(object sender, TopicCreatingEventArgs e);
}
