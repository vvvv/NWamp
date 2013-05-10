namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Message, which informs server to stop delivering messages to previously subscribed client.
    /// </summary>
    public struct UnsubscribeMessage : IWampMessage
    {
        /// <summary>
        /// URI or CURIE used to identify events published on target topic.
        /// </summary>
        public string TopicUri { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="UnsubscribeMessage"/> with provided topic.
        /// </summary>
        /// <param name="topicUri"></param>
        public UnsubscribeMessage(string topicUri) : this()
        {
            this.TopicUri = topicUri;
        }

        /// <summary>
        /// Creates new instance of <see cref="UnsubscribeMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public UnsubscribeMessage(object[] array) : this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            return new object[]
                   {
                       (int) this.Type, 
                       this.TopicUri
                   };
        }

        /// <summary>
        /// Parses provided WAMP frame to fill current instance with valid values.
        /// </summary>
        /// <param name="array"></param>
        public void FromArray(object[] array)
        {
            if (array.Length < 2)
                throw new MessageParsingException("Wrong number of array arguments", array);
            if (this.Type != (MessageTypes)(int)array[0])
                throw new MessageParsingException("Invalid array message type argument passed", array, this.Type);

            this.TopicUri = (string)array[1];
        }

        /// <summary>
        /// Type of WAMP message frame: 6.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Unsubscribe; }
        }
    }
}