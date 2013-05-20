using NWamp.Transport;
namespace NWamp.Protocol.Rpc
{
    public class CallParameters
    {
        public string CallId { get; set; }

        public string ProcUri { get; set; }

        public object[] Arguments { get; set; }

        public IWampConnection Connection { get; set; }

        public CallParameters(string callId, string procUri, object[] arguments, IWampConnection connection)
        {
            this.CallId = callId;
            this.ProcUri = procUri;
            this.Arguments = arguments;
            this.Connection = connection;
        }
    }
}
