using System.Collections;
using System.Linq;

namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Call error WAMP message, used to inform client requester about errors during RPC action invocation.
    /// </summary>
    public struct CallErrorMessage : IWampMessage
    {
        /// <summary>
        /// Gets or sets random unique string generated by client.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Gets or sets URI or CURIE identifying an error.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// Gets or sets error Description. It's intended to be understood by 
        /// developers, not end users.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets error details. When used must not be null. It's used to communicate
        /// any application's error details.
        /// </summary>
        public object[] ErrorDetails { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="CallErrorMessage"/>.
        /// </summary>
        public CallErrorMessage(string callId, string errorId, string description, params object[] args) : this()
        {
            this.CallId = callId;
            this.ErrorUri = errorId;
            this.ErrorDescription = description;
            this.ErrorDetails = args;
        }

        /// <summary>
        /// Creates new instance of <see cref="CallErrorMessage"/> based
        /// on previously parsed WAMP frame.
        /// </summary>
        public CallErrorMessage(object[] array) : this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        public object[] ToArray()
        {
            return this.ErrorDetails == null || this.ErrorDetails.Length == 0
                       ? new object[]
                         {
                             (int)this.Type,
                             this.CallId,
                             this.ErrorUri,
                             this.ErrorDescription
                         }
                       : new object[]
                         {
                             (int)this.Type,
                             this.CallId,
                             this.ErrorUri,
                             this.ErrorDescription,
                             this.ErrorDetails
                         };
        }

        /// <summary>
        /// Parses provided WAMP frame to fill current instance with valid values.
        /// </summary>
        public void FromArray(object[] array)
        {
            if (array.Length < 4)
                throw new MessageParsingException("Wrong number of array arguments", array);
            if (this.Type != (MessageTypes)(int)(long)array[0])
                throw new MessageParsingException("Invalid array message type argument passed", array, this.Type);

            this.CallId = (string) array[1];
            this.ErrorUri = (string) array[2];
            this.ErrorDescription = (string) array[3];

            if (array.Length > 4)
                this.ErrorDetails = array[4] is IEnumerable
                                        ? (array[4] as IEnumerable).Cast<object>().ToArray()
                                        : new[] {array[4]};
        }

        /// <summary>
        /// Gets or sets type of WAMP message frame: 4.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.CallError; }
        }
    }
}