using System;
using System.Collections.Generic;

namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Event arguments used in Publish/Subscribe message events.
    /// </summary>
    public abstract class PubSubEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets session identifier of message publisher.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets topic URI identifier associated with current event.
        /// </summary>
        public string TopicId { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PubSubEventArgs"/> class.
        /// </summary>
        protected PubSubEventArgs(string topicId, string sessionId)
        {
            this.TopicId = topicId;
            this.SessionId = sessionId;
        }
    }

    public delegate void PubSubEventHandler(object sender, PubSubEventArgs e);
}