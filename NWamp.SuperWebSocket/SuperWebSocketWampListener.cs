using System;
using System.Configuration;
using System.Net;
using System.Text;

using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Command;
using SuperWebSocket.SubProtocol;
using SuperWebSocket;

using NWamp.Transport;
using NWamp.Protocol;
    
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
    /// Websocket Application Message Protocol server-side listener using Alchemy websockets library.
    /// </summary>
    public class SuperWebSocketWampListener : WampListener
    {
        /// <summary>
        /// Alchemy websocket server listener.
        /// </summary>
        private readonly WebSocketServer Server;

        /// <summary>
        /// Gets or sets default connection timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening.
        /// </summary>
        public SuperWebSocketWampListener(IPAddress address, int port)
            : base()
        {
            //this.Timeout = new TimeSpan(0, 5, 0);
            var command = new WAMPSubCommand();
            
            var subprotocol = new WAMPSubProtocol(command);
            Server = new WebSocketServer(subprotocol);
            Server.Setup(port);
            
            Server.NewSessionConnected += Server_NewSessionConnected;
            command.NewDataReceived += Server_NewDataReceived;
            Server.SessionClosed += Server_SessionClosed;
        }

        void Server_SessionClosed(WebSocketSession session, CloseReason value)
        {
        	var connection = this.GetConnectionByClientAddress(session.RemoteEndPoint);
            if (connection != null)
            {
                this.OnDiconnected(connection);
            }
        }

        void Server_NewSessionConnected(WebSocketSession session)
        {
        	var connection = new SuperWebSocketWampConnection(session);
            this.OnConnected(connection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP endpoint defined for listening.
        /// </summary>
        public SuperWebSocketWampListener(IPEndPoint endPoint)
            : this(endPoint.Address, endPoint.Port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP endpoint defined for listening and methods used for objects serialization/deserialization.
        /// </summary>
        public SuperWebSocketWampListener(IPEndPoint endpoint, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : this(endpoint.Address, endpoint.Port, serializer, deserializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization.
        /// </summary>
        public SuperWebSocketWampListener(IPAddress address, int port, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : base(serializer, deserializer)
        {
            //this.Timeout = new TimeSpan(0, 5, 0);
            var command = new WAMPSubCommand();
            var subprotocol = new WAMPSubProtocol(command);
            Server = new WebSocketServer(subprotocol);
            Server.Setup(port);
            
            Server.NewSessionConnected += Server_NewSessionConnected;
            command.NewDataReceived += Server_NewDataReceived;
            Server.SessionClosed += Server_SessionClosed;
        }

        void Server_NewDataReceived(WebSocketSession session, byte[] value)
        {
        	var connection = this.GetConnectionByClientAddress(session.RemoteEndPoint);
            if (connection != null)
            {
                var json = string.Empty;
                try
                {	
                	json = Encoding.UTF8.GetString(value);
                    this.OnReceived(json, connection);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization 
        /// and object-to-object mapping.
        /// </summary>
        public SuperWebSocketWampListener(
            IPAddress address, 
            int port, 
            Func<object[], string> serializer, 
            Func<string, object[]> deserializer,
            Func<object, Type, Type, object> typeResolver)
            : base(serializer, deserializer, typeResolver)
        {
            //this.Timeout = new TimeSpan(0, 5, 0);
            var command = new WAMPSubCommand();
            var subprotocol = new WAMPSubProtocol(command);
            Server = new WebSocketServer(subprotocol);
            Server.Setup(port);

            Server.NewSessionConnected += Server_NewSessionConnected;
            command.NewDataReceived += Server_NewDataReceived;
            Server.SessionClosed += Server_SessionClosed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization 
        /// and object-to-object mapping.
        /// </summary>
        public SuperWebSocketWampListener(
            string address, 
            int port, 
            Func<object[], string> serializer, 
            Func<string, object[]> deserializer,
            Func<object, Type, Type, object> typeResolver)
        	: this(IPAddress.Parse(address), port, serializer, deserializer, typeResolver)
        {
        }
        
        

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization.
        /// </summary>
        public SuperWebSocketWampListener(string ip, int port, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : this(IPAddress.Parse(ip), port, serializer, deserializer)
        {
        }

        #endregion

        /// <summary>
        /// Starts to listening for incoming connections.
        /// </summary>
        public override void Listen()
        {
            Server.Start();
        }

        /// <summary>
        /// Disposes current web socket server.
        /// </summary>
        public override void Dispose()
        {
            Server.Stop();
            Server.Dispose();
        }

        /// <summary>
        /// Method invoked when new web socket data frame has been received.
        /// It fires proceeding of WAMP protocol message frames.
        /// </summary>
        /// <param name="context">Alchemy user context for current connection.</param>
        private void OnDataFrameReceived(WebSocketSession context)
        {
            
        }

        private static string ExtractExpectedVersion(string json)
        {
            var i = json.IndexOf("/");
            var sb = new StringBuilder(3);
            for (int j = i+1; j < json.Length; j++)
            {
                if (char.IsWhiteSpace(json[j])) break;
                sb.Append(json[j]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Method invoked when websocket connection ended or has been lost.
        /// </summary>
        /// <param name="context">Alchemy user context for current connection.</param>
        private void OnUserContextDisconnected(WebSocketSession context)
        {
            
        }

        /// <summary>
        /// Method invoked when new client connection has been received.
        /// </summary>
        /// <param name="context">Alchemy user context for current connection.</param>
        private void OnUserContextConnected(WebSocketSession context)
        {
            
        }

        /// <summary>
        /// Retrieves an Alchemy web socket user context identified by provided endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private IWampConnection GetConnectionByClientAddress(IPEndPoint endpoint)
        {
            foreach (SuperWebSocketWampConnection value in this.connections.Values)
            {
                if (value.UserContext.RemoteEndPoint == endpoint)
                    return value;
            }
            return null;
        }
    }
}
