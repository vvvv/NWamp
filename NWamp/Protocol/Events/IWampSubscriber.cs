using System.Collections.Generic;
using NWamp.Transport;
namespace NWamp.Protocol.Events
{
    /// <summary>
    /// Interface used for handling WAMP Pub/Sub messages.
    /// </summary>
    public interface IWampSubscriber
    {
        /// <summary>
        /// Subscribes target WAMP connection to topic provided.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="connection"></param>
        void Subscribe(string topicId, IWampConnection connection);

        /// <summary>
        /// Unsubscribes target WAMP connection from target topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="connection"></param>
        void Unsubscribe(string topicId, IWampConnection connection);

        /// <summary>
        /// Publishes WAMP event object amongs subscribers associated with current topic.
        /// </summary>
        /// <param name="topicId">Topic URI identifier.</param>
        /// <param name="sender">Event sender's session.</param>
        /// <param name="evt">Event object.</param>
        /// <param name="eligible">
        /// List of eligible receivers. If this param is not null 
        /// or empty, excludes list and excludeMe flag will be ignored.
        /// </param>
        /// <param name="excludes">
        /// List of excluded receivers (event will not be emitted to them).
        /// </param>
        /// <param name="excludeMe">Should sender be excluded?</param>
        /// <returns></returns>
        void Publish(string topicId, string senderSession, object evt, IEnumerable<string> eligible, IEnumerable<string> excludes, bool excludeMe);
    }
}