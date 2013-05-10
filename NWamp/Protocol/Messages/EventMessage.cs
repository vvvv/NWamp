namespace NWamp.Protocol.Messages
{
    using NWamp.Protocol.Events;

    /// <summary>
    /// Event message send on publisher's request by server to all topic subscribers.
    /// </summary>
    public struct EventMessage : IWampMessage
    {
        /// <summary>
        /// Gets or sets URI or CURIE used to identify events published on target topic.
        /// </summary>
        public string TopicUri { get; set; }

        /// <summary>
        /// Gets or sets object send by publisher to all subscribers.
        /// </summary>
        public object Event { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="EventMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public EventMessage(object[] array):this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Creates new instance of <see cref="EventMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public EventMessage(string topicId, object evt)
            : this()
        {
            this.TopicUri = topicId;
            this.Event = evt;
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            return new object[]
                   {
                       (int)this.Type,
                       this.TopicUri,
                       this.Event
                   };
        }

        /// <summary>
        /// Parses provided WAMP frame to fill current instance with valid values.
        /// </summary>
        /// <param name="array"></param>
        public void FromArray(object[] array)
        {
            if (array.Length < 3)
                throw new MessageParsingException("Wrong number of array arguments", array);
            if (this.Type != (MessageTypes)(int)array[0])
                throw new MessageParsingException("Invalid array message type argument passed", array, this.Type);

            this.TopicUri = (string) array[1];
            this.Event = array[2];
        }

        /// <summary>
        /// Gets or sets type of WAMP message frame: 8.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Event; }
        }
    }
}