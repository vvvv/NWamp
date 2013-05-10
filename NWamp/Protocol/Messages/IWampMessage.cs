namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Common interface for all WAMP messages.
    /// </summary>
    public interface IWampMessage
    {
        /// <summary>
        /// Serializes current message to array. This is later serializes into JSON string.
        /// </summary>
        object[] ToArray();

        /// <summary>
        /// Fills current message values from deserialized JSON array.
        /// </summary>
        void FromArray(object[] array);

        /// <summary>
        /// Gets message type.
        /// </summary>
        MessageTypes Type { get; }
    }
}