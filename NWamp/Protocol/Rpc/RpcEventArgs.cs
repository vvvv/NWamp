using System;

namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Event args used in RPC calls event handling.
    /// </summary>
    public class RpcEventArgs: EventArgs
    {
        /// <summary>
        /// Call identifier associated with current event.
        /// </summary>
        public string CallId { get; set; }

        /// <summary>
        /// Procedure URI of current RPC call.
        /// </summary>
        public string ProcUri { get; set; }

        /// <summary>
        /// Session identifier of RPC caller.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Determines if current RPC action has ended succesfully.
        /// If action is still pending, this value is null.
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// Gets optional exception, if any has occurred during RPC action execution.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="RpcEventArgs"/> class. 
        /// Use this construction to signalize call events, that have not yet ended.
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="procUri"></param>
        /// <param name="sessionId"></param>
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
        /// <param name="callId"></param>
        /// <param name="procUri"></param>
        /// <param name="sessionId"></param>
        /// <param name="exc"></param>
        public RpcEventArgs(string callId, string procUri, string sessionId, Exception exc) : this(callId, procUri, sessionId)
        {
            this.IsSuccess = exc == null;
            this.Error = exc;
        }
    }

    public delegate void RpcEventHandler(object sender, RpcEventArgs e);
}
