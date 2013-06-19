using System;
using System.Collections.Generic;

namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Class representing single WAMP topic.
    /// </summary>
    public class Topic : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Topic"/> class.
        /// </summary>
        /// <param name="topicId">URI topic identifier.</param>
        public Topic(string topicId)
        {
            this.TopicId = topicId;
            this.Sessions = new HashSet<string>();
        }

        /// <summary>
        /// Gets current <see cref="Topic"/> identifier.
        /// </summary>
        public string TopicId { get; protected set; }

        /// <summary>
        /// Gets collection of client sessions subscribed to current <see cref="Topic"/>.
        /// </summary>
        public HashSet<string> Sessions { get; protected set; }

        /// <summary>
        /// Gets value indicating if current <see cref="Topic"/> has no subscribers.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Sessions.Count == 0; }
        }

        /// <summary>
        /// Subscribes given <paramref name="session"/> to current <see cref="Topic"/>.
        /// </summary>
        /// <param name="session">Session identifier.</param>
        public void Subscribe(string session)
        {
            this.Sessions.Add(session);
        }

        /// <summary>
        /// Unsubscribes given <paramref name="session"/> from current <see cref="Topic"/>.
        /// </summary>
        /// <param name="session">Session identifier.</param>
        public void Unsubscribe(string session)
        {
            this.Sessions.Remove(session);
        }

        /// <summary>
        /// Closes current topic, clearing all subscribers.
        /// </summary>
        public void Close()
        {
            this.Sessions.Clear();
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }
    }
}
