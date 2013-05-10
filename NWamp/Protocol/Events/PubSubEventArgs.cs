using System;
using System.Collections.Generic;

namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments used in Publish/Subscribe message events.
    /// </summary>
    public class PubSubEventArgs : EventArgs
    {
        /// <summary>
        /// Session identifier of message publisher.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Topic URI identifier associated with current event.
        /// </summary>
        public string TopicId { get; set; }

        /// <summary>
        /// List of event receivers sessions ids.
        /// </summary>
        public IEnumerable<string> Receivers { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PubSubEventArgs"/> class.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="senderSessionId"></param>
        /// <param name="receivers"></param>
        public PubSubEventArgs(string topicId, string senderSessionId, IEnumerable<string> receivers)
        {
            this.TopicId = topicId;
            this.SessionId = senderSessionId;
            this.Receivers = receivers;
        }
    }

    public delegate void PubSubEventHandler(object sender, PubSubEventArgs e);
}