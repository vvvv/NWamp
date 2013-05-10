using System.Collections.Generic;

namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Initial WAMP message for RPC connection.
    /// </summary>
    public struct CallMessage : IWampMessage
    {
        /// <summary>
        /// Random unique string generated by client.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Remote procedure URI to be called.
        /// </summary>
        public string ProcUri { get; set; }

        /// <summary>
        /// Optional arguments used for establishing connection.
        /// </summary>
        public IList<object> Arguments { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="CallMessage"/> class.
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="procUri"></param>
        /// <param name="args"></param>
        public CallMessage(string callId, string procUri, params object[] args) : this()
        {
            this.CallId = callId;
            this.ProcUri = procUri;
            this.Arguments = args ?? new object[0];
        }

        /// <summary>
        /// Creates new instance of <see cref="CallMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        /// <param name="array"></param>
        public CallMessage(object[] array):this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            var array = new object[3 + this.Arguments.Count];
            array[0] = (int) this.Type;
            array[1] = this.CallId;
            array[2] = this.ProcUri;

            for (int i = 0; i < this.Arguments.Count; i++)
                array[i + 3] = this.Arguments[i];

            return array;
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

            this.CallId = (string) array[1];
            this.ProcUri = (string) array[2];
            this.Arguments = new List<object>(array.Length - 3);

            for (int i = 3; i < array.Length; i++)
                this.Arguments.Add(array[i]);
        }

        /// <summary>
        /// Type of WAMP message frame: 2.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Call; }
        }
    }
}