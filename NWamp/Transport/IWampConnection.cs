using NWamp.Mapping;

namespace NWamp.Transport
{
    /// <summary>
    /// Interface representing single WAMP client-server connection.
    /// </summary>
    public interface IWampConnection
    {
        string SessionId { get; set; }
        PrefixMap Prefixes { get; }

        void SendMessage(string json);
    }
}