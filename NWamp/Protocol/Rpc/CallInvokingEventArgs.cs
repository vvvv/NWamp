namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Event arguments used before call invoke will finish.
    /// </summary>
    public class CallInvokingEventArgs : RpcEventArgs
    {
        public bool Cancel { get; set; }

        public CallInvokingEventArgs(string callId, string procUri, string sessionId)
            : base(callId, procUri, sessionId)
        {
        }
    }

    public delegate void CallInvokingEventHandler(object sender, CallInvokingEventArgs e);
}
