namespace NWamp.Protocol.Events
{
    using System.Collections.Generic;

    /// <summary>
    /// Event arguments used before new event will be published.
    /// </summary>
    public class PublishingEventArgs
    {
        public ICollection<string> ReceiversSessions { get; set; }

        public PublishingEventArgs(string topicId, string sessionId, IEnumerable<string> receivers)
        {
            this.ReceiversSessions = new List<string>(receivers);
        }
    }

    public delegate void PublishingEventHandler(object sender, PublishingEventArgs e);
}
