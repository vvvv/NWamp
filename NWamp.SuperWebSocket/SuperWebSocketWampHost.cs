using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;

using SuperSocket.SocketBase;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;

using NWamp.SuperWebSocket;
using NWamp.Transport;

namespace NWamp
{
    public class WAMPSubCommand: ISubCommand<WebSocketSession>
	{
		public string Name 
		{ 
			get {return "wamp";}
		}
		
		public Action<WebSocketSession, byte[]> NewDataReceived;
		
		public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
		{
			NewDataReceived.Invoke(session, Encoding.UTF8.GetBytes(requestInfo.Key));
		}
	}

	public class WAMPSubProtocol: BasicSubProtocol
	{
		private WAMPSubCommand Command;
		
		public WAMPSubProtocol(WAMPSubCommand command)
			:base("wamp")
		{
			Command = command;
		}
		
		public override bool TryGetCommand(string name, out ISubCommand<WebSocketSession> command)
        {
			command = Command;
            return true;
        }
	}

    /// <summary>
    /// Concrete implementation of WAMP server host using SuperWebSocket web sockets.
    /// </summary>
    public class SuperWebSocketWampHost : BaseWampHost, IDisposable
    {
        /// <summary>
        /// Additional mapper used for mapping SuperWebSocket <see cref="UserContext"/> objects to <see cref="IWampSession"/>.
        /// </summary>
        private readonly IDictionary<EndPoint, IWampSession> _endpointSessions;

        /// <summary>
        /// SuperWebSocket web socket server object.
        /// </summary>
        private readonly WebSocketServer _server;

        /// <summary>
        /// Gets an SuperWebSocket web socket server object.
        /// </summary>
        public WebSocketServer Server { get { return _server; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperWebSocketWampHost"/> class.
        /// </summary>
        /// <param name="address">IP address for current host</param>
        /// <param name="port">Listening port</param>
        public SuperWebSocketWampHost(IPAddress address, int port)
            : base(new Uri("ws://" + address + ":" + port), new NewtonsoftWampMessageProvider(), new NewtonsoftTypeResolver())
        {
            _endpointSessions = new Dictionary<EndPoint, IWampSession>();
            
            var command = new WAMPSubCommand();
            
            var subprotocol = new WAMPSubProtocol(command);
            _server = new WebSocketServer(subprotocol);
            _server.Setup(port);
            
            _server.NewSessionConnected += Server_NewSessionConnected;
            command.NewDataReceived += Server_NewDataReceived;
            _server.SessionClosed += Server_SessionClosed;
        }
        
        void Server_NewSessionConnected(WebSocketSession context)
        {
        	var connection = new SuperWebSocketWampConnection(context);
            this.OnConnected(connection);
        }
        
        void Server_NewDataReceived(WebSocketSession context, byte[] value)
        {
            var json = string.Empty;
            try
            {	
            	json = Encoding.UTF8.GetString(value);
                this.OnReceive(context, json);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        void Server_SessionClosed(WebSocketSession context, CloseReason value)
        {
        	this.OnDisconnect(context);
        }

        /// <summary>
        /// Starts listening, waiting for incoming connections.
        /// </summary>
        public override void Start()
        {
            base.Start();
            _server.Start();
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            Dispose();
            _server.Stop();
        }

        /// <summary>
        /// Disposes current host, closing all sessions.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _server.Dispose();
        }

        /// <summary>
        /// Method called when new web socket client has connected.
        /// </summary>
        /// <param name="context">SuperWebSocket context for incoming clients</param>
        private void OnConnected(WebSocketSession context)
        {
            var connection = new SuperWebSocketWampConnection(context);
            OnConnected(connection);
        }

        /// <summary>
        /// Method called when new message from client has been received.
        /// </summary>
        /// <param name="context">SuperWebSocket web socket client context</param>
        protected virtual void OnReceive(WebSocketSession context, string json)
        {
            var session = GetSession(context);
            ReceiveJson(session, json);
        }

        /// <summary>
        /// Method called when new message has been send to client.
        /// </summary>
        /// <param name="context">SuperWebSocket web socket client context</param>
        private void OnSend(WebSocketSession context)
        {
            //TODO: ???
        }

        /// <summary>
        /// Method called when existing web socket client has disconnected.
        /// </summary>
        /// <param name="context">SuperWebSocket web socket client context</param>
        private void OnDisconnect(WebSocketSession context)
        {
            var session = GetSession(context);
            _endpointSessions.Remove(context.RemoteEndPoint);

            OnDisconnected(session);
        }
             
        /// <summary>
        /// Returns WAMP session object for provided <see cref="UserContext"/> instance.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IWampSession GetSession(WebSocketSession context)
        {
            IWampSession session;
            return _endpointSessions.TryGetValue(context.RemoteEndPoint, out session) ? session : null;
        }

        /// <summary>
        /// Initalizes a new WAMP session.
        /// </summary>
        /// <param name="connection"><see cref="SuperWebSocketWampConnection"/> instance</param>
        /// <returns></returns>
        protected override IWampSession InitializeWampSession(IWampConnection connection)
        {
            var swsConnection = connection as SuperWebSocketWampConnection;
            if (swsConnection == null)
            {
                throw new ArgumentException("Provided WAMP connection was not of type SuperWebSocketWampConnection");
            }

            var session = base.InitializeWampSession(connection);
            _endpointSessions.Add(swsConnection.Context.RemoteEndPoint, session);

            return session;
        }
    }
}
