using System;
namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Exception raised when a WAMP arguments (JSON array) parsing
    /// to NWamp message fails.
    /// </summary>
    public class MessageParsingException : WampException
    {
        /// <summary>
        /// Gets or sets original JSON array of parsed values received from WAMP message.
        /// </summary>
        public object Object { get; set; }
        
        /// <summary>
        /// Gets or sets expected (optional) WAMP message type.
        /// </summary>
        public MessageTypes? ExpectedType { get; set; }

        public MessageParsingException(string message): base(message)
        {
        }

        public MessageParsingException(string message, object jsonObject) : base(message)
        {
            this.Object = jsonObject;
        }

        public MessageParsingException(string message, object jsonObject, MessageTypes expected) :
            base(message)
        {
            this.Object = jsonObject;
            this.ExpectedType = expected;
        }
    }
}