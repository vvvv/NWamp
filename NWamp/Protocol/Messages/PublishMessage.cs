namespace NWamp.Protocol.Messages
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Event send by client to all clients connected to server who have subscribed
    /// to specific topic.
    /// </summary>
    public struct PublishMessage : IWampMessage
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
        /// Gets or sets value determining if event publisher should be excluded from receiving an event.
        /// </summary>
        public bool ExcludeMe { get; set; }

        /// <summary>
        /// Set of client identifiers used to inform server, which subscribers 
        /// should be excluded from receiving an event.
        /// </summary>
        public IList<string> Excludes { get; set; }

        /// <summary>
        /// Gets or sets collection of client identifiers used to inform server, which subscribers 
        /// are eligible to receive an event.
        /// </summary>
        public IList<string> Eligibles { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PublishMessage"/> class.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="evt"></param>
        public PublishMessage(string topicId, object evt) : this()
        {
            this.TopicUri = topicId;
            this.Event = evt;
        }

        /// <summary>
        /// Creates new instance of <see cref="PublishMessage"/> class.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="evt"></param>
        /// <param name="excludeSender"></param>
        public PublishMessage(string topicId, object evt, bool excludeSender)
            : this(topicId, evt)
        {
            this.ExcludeMe = excludeSender;
        }

        /// <summary>
        /// Creates new instance of <see cref="PublishMessage"/> class.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="evt"></param>
        /// <param name="eligibles"></param>
        /// <param name="excludes"></param>
        public PublishMessage(string topicId, object evt, IList<string> eligibles, IList<string> excludes)
            : this(topicId, evt)
        {
            this.Eligibles = eligibles ?? new List<string>();
            this.Excludes = excludes ?? new List<string>();
        }

        /// <summary>
        /// Creates new instance of <see cref="PublishMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public PublishMessage(object[] array):this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            if (this.Excludes.Count > 0 || this.Eligibles.Count >0)
            {
                return new object[]
                       {
                           (int)this.Type,
                           this.TopicUri,
                           this.Event,
                           this.Excludes.ToArray(),
                           this.Eligibles.ToArray()
                       };
            }
            else if (this.ExcludeMe)
            {
                return new object[]
                       {
                           (int)this.Type,
                           this.TopicUri,
                           this.Event,
                           this.ExcludeMe
                       };
            }
            else
            {
                return new object[]
                       {
                           (int)this.Type,
                           this.TopicUri,
                           this.Event
                       };
            }
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

            if (array.Length == 4)
                this.ExcludeMe = (bool) array[3];
            if (array.Length == 5)
            {
                this.Excludes = new List<string>((string[])array[3]);
                this.Eligibles = new List<string>((string[])array[4]);
            }
            else
            {
                this.Excludes = new List<string>();
                this.Eligibles = new List<string>();
            }
        }

        /// <summary>
        /// Gets or sets type of WAMP message frame: 7.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Publish; }
        }
    }
}