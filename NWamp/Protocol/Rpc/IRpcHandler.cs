using System;
namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Interface used for handling WAMP RPC connections.
    /// </summary>
    public interface IRpcHandler
    {
        object Call(string callId, string procId, params object[] args);

        void RegisterRpcAction(string procId, Func<object[], object> func);

        void UnregisterRpcAction(string procId);

    }
}