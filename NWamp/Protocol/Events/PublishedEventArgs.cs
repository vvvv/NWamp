namespace NWamp.Protocol.Events
{
    using System.Collections.Generic;

    /// <summary>
    /// Event arguments class created when new message has been published.
    /// </summary>
    public class PublishedEventArgs : PubSubEventArgs
    {
        public IEnumerable<string> ReceiversSessions { get; set; }

        public PublishedEventArgs(string topicId, string sessionId, IEnumerable<string> receivers)
            : base(topicId, sessionId)
        {
            this.ReceiversSessions = receivers;
        }
    }

    public delegate void PublishedEventHandler (object sender, PubSubEventArgs e);
}
