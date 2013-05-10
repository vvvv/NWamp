using System;
using System.Collections;
using System.Linq;

namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Call error exception thrown when an exception occurred in RPC action.
    /// </summary>
    public class CallErrorException : WampException
    {
        /// <summary>
        /// RPC call identifier.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// URI or CURIE identifing an error.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// Error description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional details associated with error.
        /// </summary>
        public object[] Details { get; set; }

        public CallErrorException(string callId, string errorUri, Exception innerException)
            : this(callId, errorUri, innerException.Message)
        {
            if (innerException.Data != null)
            {
                var args = new object[innerException.Data.Count];
                int i = 0;
                foreach (DictionaryEntry entry in innerException.Data)
                {
                    args[i++] = new[] {entry.Key, entry.Value};
                }
                this.Details = args;
            }
        }

        public CallErrorException(string callId, string errorUri, string errorDesc, 
            params object[] details) : base(errorDesc)
        {
            this.CallId = callId;
            this.ErrorUri = errorUri;
            this.Description = errorDesc;
            this.Details = details;
        }

        
    }
}