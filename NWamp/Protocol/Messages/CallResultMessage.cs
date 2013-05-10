namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public struct CallResultMessage : IWampMessage
    {
        /// <summary>
        /// Random unique string generated by client.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Result send by server as response for client RPC connection initialization.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="CallResultMessage"/>.
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="result"></param>
        public CallResultMessage(string callId, object result) : this()
        {
            this.CallId = callId;
            this.Result = result;
        }

        /// <summary>
        /// Creates new instance of <see cref="CallResultMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public CallResultMessage(object[] array):this()
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
                       (int)this.Type,
                       this.CallId,
                       this.Result
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

            this.CallId = (string)array[1];
            this.Result = array[2];
        }

        /// <summary>
        /// Type of WAMP message frame: 3.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.CallResult; }
        }
    }
}