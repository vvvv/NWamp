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
        /// Original JSON array of parsed values received from WAMP message.
        /// </summary>
        public object[] JsonArray { get; set; }
        
        /// <summary>
        /// Expected (optional) WAMP message type.
        /// </summary>
        public MessageTypes? ExpectedType { get; set; }

        public MessageParsingException(string message): base(message)
        {
        }

        public MessageParsingException(string message, object[] jsonArray) : base(message)
        {
            this.JsonArray = jsonArray;
        }

        public MessageParsingException(string message, object[] jsonArray, MessageTypes expected):
            base(message)
        {
            this.JsonArray = jsonArray;
            this.ExpectedType = expected;
        }
    }
}