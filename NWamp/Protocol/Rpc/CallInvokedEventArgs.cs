using System;
namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Event arguments used after RPC call has finished or failed.
    /// </summary>
    public class CallInvokedEventArgs : RpcEventArgs
    {
        public CallInvokedEventArgs(string callId, string procUri, string sessionId, Exception exc)
            : base(callId, procUri, sessionId, exc)
        {
        }
    }

    public delegate void CallInvokedEventHandler(object sender, CallInvokedEventArgs e);
}
