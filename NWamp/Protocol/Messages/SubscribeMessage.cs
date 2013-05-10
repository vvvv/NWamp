namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Client's request access to receive events published.
    /// </summary>
    public struct SubscribeMessage : IWampMessage
    {
        /// <summary>
        /// Gets or sets URI or CURIE used to identify events published on target topic.
        /// </summary>
        public string TopicUri { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="SubscribeMessage"/> with provided topic.
        /// </summary>
        /// <param name="topicUri"></param>
        public SubscribeMessage(string topicUri) : this()
        {
            this.TopicUri = topicUri;
        }

        /// <summary>
        /// Creates new instance of <see cref="SubscribeMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public SubscribeMessage(object[] array)
            : this()
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
        /// Gets or sets type of WAMP message frame: 5.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Subscribe; }
        }
    }
}