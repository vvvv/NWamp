using System;
namespace NWamp.Protocol.Rpc
{
    public interface ICallable
    {
        object Call(string callId, string procId, TimeSpan timeout, params object[] args);
    }

    public interface ICallable<out TResult> : ICallable
    {
        TResult Call(string callId, string procId, TimeSpan timeout, params object[] args);
    }
}