namespace NWamp.Transport
{
    /// <summary>
    /// WebSocket interface. Implement specific version based on websocket library required.
    /// </summary>
    public interface IWebSocket
    {
        void OnConnect();

        void OnDisconnect();

        void SendMessage(string json);

        string ReceiveMessage();
    }
}