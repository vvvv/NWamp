namespace NWamp.Transport
{
    /// <summary>
    /// WebSocket interface. Implement specific version based on websocket library required.
    /// </summary>
    public interface IWebSocket
    {
        /// <summary>
        /// Method invoked when new connection has been established.
        /// </summary>
        void OnConnect();

        /// <summary>
        /// Method invoked on connection lost.
        /// </summary>
        void OnDisconnect();

        /// <summary>
        /// Method used to send (expected) stringified JSON message. WAMP protocol requires only
        /// text-based web socket message sending.
        /// </summary>
        void SendMessage(string json);

        /// <summary>
        /// Method invoked when new message has been received. 
        /// </summary>
        /// <returns> Returns (expected) stringified JSON message.</returns>
        string ReceiveMessage();
    }
}