namespace NWamp.Protocol.Rpc
{
    using System;

    /// <summary>
    /// Event args class used in RPC calls event handling.
    /// </summary>
    public class RpcEventArgs: EventArgs
    {
        /// <summary>
        /// Gets or sets call identifier associated with current event.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Gets or sets procedure URI identifier of current RPC call.
        /// </summary>
        public string ProcUri { get; set; }

        /// <summary>
        /// Gets or sets session identifier of RPC caller.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets value, which determines if current RPC action has ended succesfully.
        /// If action is still pending, this value is null.
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets optional exception, if any has occurred during RPC action execution.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="RpcEventArgs"/> class. 
        /// Use this construction to signalize call events, that have not yet ended.
        /// </summary>
        public RpcEventArgs(string callId, string procUri, string sessionId)
        {
            this.CallId = callId;
            this.ProcUri = procUri;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Creates new instance of <see cref="RpcEventArgs"/> class. 
        /// Use this construction to signalize call events, that have ended already.
        /// </summary>
        public RpcEventArgs(string callId, string procUri, string sessionId, Exception exc) : this(callId, procUri, sessionId)
        {
            this.IsSuccess = exc == null;
            this.Error = exc;
        }
    }

    public delegate void RpcEventHandler(object sender, RpcEventArgs e);
}
