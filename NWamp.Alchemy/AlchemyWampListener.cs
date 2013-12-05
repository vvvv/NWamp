namespace NWamp
{
    using System;
    using System.Net;
    using Alchemy;
    using Alchemy.Classes;
    using NWamp.Transport;
    using System.Text;
    using NWamp.Protocol;

    /// <summary>
    /// Websocket Application Message Protocol server-side listener using Alchemy websockets library.
    /// </summary>
    public class AlchemyWampListener : WampListener
    {
        /// <summary>
        /// Alchemy websocket server listener.
        /// </summary>
        private readonly WebSocketServer server;

        /// <summary>
        /// Gets or sets default connection timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening.
        /// </summary>
        public AlchemyWampListener(IPAddress address, int port)
            : base()
        {
            this.Timeout = new TimeSpan(0, 5, 0);
            this.server = new WebSocketServer(port, address)
            {
                TimeOut = this.Timeout,
                OnConnected = OnUserContextConnected,
                OnDisconnect = OnUserContextDisconnected,
                OnReceive = OnDataFrameReceived
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP endpoint defined for listening.
        /// </summary>
        public AlchemyWampListener(IPEndPoint endPoint)
            : this(endPoint.Address, endPoint.Port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP endpoint defined for listening and methods used for objects serialization/deserialization.
        /// </summary>
        public AlchemyWampListener(IPEndPoint endpoint, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : this(endpoint.Address, endpoint.Port, serializer, deserializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization.
        /// </summary>
        public AlchemyWampListener(IPAddress address, int port, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : base(serializer, deserializer)
        {
            this.Timeout = new TimeSpan(0, 5, 0);
            this.server = new WebSocketServer(port, address)
            {
                TimeOut = this.Timeout,
                OnConnected = OnUserContextConnected,
                OnDisconnect = OnUserContextDisconnected,
                OnReceive = OnDataFrameReceived
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization 
        /// and object-to-object mapping.
        /// </summary>
        public AlchemyWampListener(
            IPAddress address, 
            int port, 
            Func<object[], string> serializer, 
            Func<string, object[]> deserializer,
            Func<object, Type, Type, object> typeResolver)
            : base(serializer, deserializer, typeResolver)
        {
            this.Timeout = new TimeSpan(0, 5, 0);
            this.server = new WebSocketServer(port, address)
            {
                TimeOut = this.Timeout,
                OnConnected = OnUserContextConnected,
                OnDisconnect = OnUserContextDisconnected,
                OnReceive = OnDataFrameReceived
            };
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AlchemyWampListener"/> class
        /// with IP address and port defined for listening, with methods used for objects serialization/deserialization 
        /// and object-to-object mapping.
        /// </summary>
        public AlchemyWampListener(
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
        public AlchemyWampListener(string ip, int port, Func<object[], string> serializer, Func<string, object[]> deserializer)
            : this(IPAddress.Parse(ip), port, serializer, deserializer)
        {
        }

        #endregion

        /// <summary>
        /// Starts to listening for incoming connections.
        /// </summary>
        public override void Listen()
        {
            this.server.Start();
        }

        /// <summary>
        /// Disposes current web socket server.
        /// </summary>
        public override void Dispose()
        {
            this.server.Stop();
            this.server.Dispose();
        }

        /// <summary>
        /// Method invoked when new web socket data frame has been received.
        /// It fires proceeding of WAMP protocol message frames.
        /// </summary>
        /// <param name="context">Alchemy user context for current connection.</param>
        private void OnDataFrameReceived(UserContext context)
        {
            var connection = this.GetConnectionByClientAddress(context.ClientAddress);
            if (connection != null)
            {
                var json = string.Empty;
                try
                {
                    json = context.DataFrame.ToString();
                    this.OnReceived(json, connection);
                }
                catch (Exception)
                {
                    throw;
                }
            }
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
        private void OnUserContextDisconnected(UserContext context)
        {
            var connection = this.GetConnectionByClientAddress(context.ClientAddress);
            if (connection != null)
            {
                this.OnDiconnected(connection);
            }
        }

        /// <summary>
        /// Method invoked when new client connection has been received.
        /// </summary>
        /// <param name="context">Alchemy user context for current connection.</param>
        private void OnUserContextConnected(UserContext context)
        {
            var connection = new AlchemyWampConnection(context);
            this.OnConnected(connection);
        }

        /// <summary>
        /// Retrieves an Alchemy web socket user context identified by provided endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private IWampConnection GetConnectionByClientAddress(EndPoint endpoint)
        {
            foreach (AlchemyWampConnection value in this.connections.Values)
            {
                if (value.UserContext.ClientAddress == endpoint)
                    return value;
            }
            return null;
        }
    }
}
