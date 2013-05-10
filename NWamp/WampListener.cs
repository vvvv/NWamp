using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NWamp.Protocol.Events;
using NWamp.Protocol.Messages;
using NWamp.Protocol.Rpc;
using NWamp.Transport;
using System;
using System.Threading.Tasks;
using NWamp.Mapping;

namespace NWamp
{
    /// <summary>
    /// Server-side listener using WAMP protocol.
    /// </summary>
    public class WampListener : BaseWampProtocol, IRpcHandler, IWampSubscriber, IDisposable
    {
        #region Properties & Fields

        /// <summary>
        /// Dictionary containing list of actualy 
        /// connected clients recognized by their session ids.
        /// </summary>
        protected readonly IDictionary<string, IWampConnection> connections;

        /// <summary>
        /// Map of topics and their subscribers sessions.
        /// </summary>
        protected readonly IDictionary<string, HashSet<string>> topics;

        /// <summary>
        /// Map of procedures associated with RPC action calls.
        /// </summary>
        protected readonly IDictionary<string, Func<object[], object>> procedures;

        /// <summary>
        /// List of RPC calls realized at the moment.
        /// </summary>
        private readonly ConcurrentDictionary<string, Task> calls;

        /// <summary>
        /// Collection of RPC calls realized at the moment.
        /// </summary>
        public IDictionary<string, Task> CurrentCalls
        {
            get { return this.calls; }
        }

        /// <summary>
        /// Determines, if listener should block creating topics on client's demand.
        /// </summary>
        public bool FixedTopics { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event raised, when new session has been created.
        /// </summary>
        public event SessionEventHandler SessionCreated;

        /// <summary>
        /// Event raised, when existing session has expired, closed or disconnected.
        /// </summary>
        public event SessionEventHandler SessionClosed;

        /// <summary>
        /// Event fired, when new event has been published.
        /// </summary>
        public event PubSubEventHandler EventPublished;

        /// <summary>
        /// Event raised when new RPC call has been received and started to work.
        /// </summary>
        public event RpcEventHandler CallInvoking;

        /// <summary>
        /// Event raised when RPC call has ended.
        /// </summary>
        public event RpcEventHandler CallInvoked;

        #endregion

        /// <summary>
        /// Create new instance of <see cref="WampListener"/>.
        /// </summary>
        public WampListener()
            : this(new DefaultJsonSerializer())
        {
        }

        /// <summary>
        /// Create new instance of <see cref="WampListener"/>.
        /// </summary>
        /// <param name="serializer"></param>
        public WampListener(IJsonSerializer serializer) 
            : base(serializer)
        {
            this.connections = new Dictionary<string, IWampConnection>();
            this.topics = new Dictionary<string, HashSet<string>>();
            this.procedures = new Dictionary<string, Func<object[], object>>();
            this.calls = new ConcurrentDictionary<string, Task>();
        }

        /// <summary>
        /// Method invoked when new connection has been established.
        /// </summary>
        public virtual void OnConnected(IWampConnection connection)
        {
            var sessionId = this.GenerateSessionKey();
            connection.SessionId = sessionId;
            this.connections.Add(sessionId, connection);

            var welcome = new WelcomeMessage(sessionId, 
                WampConfiguration.ProtocolVersion,
                WampConfiguration.Implementation);

            var json = this.SerializeMessage(welcome);
            connection.SendMessage(json);

            if (this.SessionCreated != null)
                this.SessionCreated(this, new SessionEventArgs(sessionId));
        }

        /// <summary>
        /// Method invoked when connection has been lost or closed.
        /// </summary>
        /// <param name="connection"></param>
        public virtual void OnDiconnected(IWampConnection connection)
        {
            if (this.connections.ContainsKey(connection.SessionId))
            {
                this.connections.Remove(connection.SessionId);

                if (this.SessionClosed != null)
                    this.SessionClosed(this, new SessionEventArgs(connection.SessionId));
            }
        }

        /// <summary>
        /// Method invoked when connection received a string message.
        /// </summary>
        /// <param name="connection"></param>
        public virtual void OnReceived(string json, IWampConnection connection)
        {
            var array = this.Serializer.DeserializeArray(json);
            var msg = MessageMapper.Get(array);

            this.OnMessageReceived(msg, connection);
        }

        #region Message responding

        /// <summary>
        /// Method invoked when new message has been received.
        /// </summary>
        protected bool OnMessageReceived(IWampMessage message, IWampConnection connection)
        {
            switch (message.Type)
            {
                case MessageTypes.Prefix:
                    OnPrefixMessage((PrefixMessage)message, connection);
                    return true;
                case MessageTypes.Call:
                    OnCallMessage((CallMessage)message, connection);
                    return true;
                case MessageTypes.Subscribe:
                    OnSubscribeMessage((SubscribeMessage)message, connection);
                    return true;
                case MessageTypes.Unsubscribe:
                    OnUnsubscribeMessage((UnsubscribeMessage)message, connection);
                    return true;
                case MessageTypes.Publish:
                    OnPublishMessage((PublishMessage)message, connection);
                    return true;
            }
            // other message types are not handled by server
            return false;
        }

        /// <summary>
        /// Method invoked when new <see cref="PublishMessage"/> has been received.
        /// </summary>
        protected void OnPublishMessage(PublishMessage msg, IWampConnection connection)
        {
            var topicId = connection.Prefixes.Map(msg.TopicUri);
            this.Publish(topicId, connection.SessionId, msg.Event, msg.Eligibles, msg.Excludes, msg.ExcludeMe);
        }

        /// <summary>
        /// Method invoked when new <see cref="UnsubscribeMessage"/> has been received.
        /// </summary>
        protected void OnUnsubscribeMessage(UnsubscribeMessage msg, IWampConnection connection)
        {
            var topicUri = connection.Prefixes.Map(msg.TopicUri);
            this.Unsubscribe(topicUri, connection);
        }

        /// <summary>
        /// Method invoked when new <see cref="SubscribeMessage"/> has been received.
        /// </summary>
        protected void OnSubscribeMessage(SubscribeMessage msg, IWampConnection connection)
        {
            var topicUri = connection.Prefixes.Map(msg.TopicUri);
            this.Subscribe(topicUri, connection);
        }

        /// <summary>
        /// Method invoked when new <see cref="CallMessage"/> has been received.
        /// </summary>
        protected void OnCallMessage(CallMessage msg, IWampConnection connection)
        {
            var callTask = Task.Factory.StartNew(obj =>
            {
                var a = obj as object[];
                var callId = (string)a[0];
                var procId = (string)a[1];
                var args = (IEnumerable<object>)a[2];
                var conn = (IWampConnection)a[3];

                Exception exception = null;
                var procUri = conn.Prefixes.Map(procId);
                try
                {
                    var result = this.Call(callId, procUri, args.ToArray());
                    var callResponse = new CallResultMessage(callId, result);
                    var json = this.Serializer.SerializeArray(callResponse.ToArray());
                    connection.SendMessage(json);
                } // we can react on inner RPC procedures exception if they are thrown as   CallErrorExceptions
                catch (CallErrorException exc)
                {
                    var errorUri = (conn.Prefixes.ContainsMapping(exc.ErrorUri))
                                       ? conn.Prefixes.Map(exc.ErrorUri)
                                       : exc.ErrorUri;

                    var callError = new CallErrorMessage(exc.CallId, errorUri, exc.Description, exc.Details);
                    var json = this.Serializer.SerializeArray(callError.ToArray());
                    connection.SendMessage(json);
                }
                catch (Exception exc)
                {
                    exception = exc;
                    throw;
                }
                finally
                {
                    Task t;
                    this.calls.TryRemove(callId, out t);

                    if (this.CallInvoked != null)
                        this.CallInvoked(this,
                            new RpcEventArgs(callId, procUri, conn.SessionId, exception));
                }
            }, new object[] { msg.CallId, msg.ProcUri, msg.Arguments, connection });

            this.calls.TryAdd(msg.CallId, callTask);

            if(this.CallInvoking != null)
                this.CallInvoking(this, new RpcEventArgs(msg.CallId, 
                    connection.Prefixes.Map(msg.ProcUri), connection.SessionId));
        }

        /// <summary>
        /// Method invoked when new <see cref="PrefixMessage"/> has been received.
        /// Prefixes are used to define new CURIE-to-URI mappings.
        /// </summary>
        protected void OnPrefixMessage(PrefixMessage msg, IWampConnection connection)
        {
            connection.Prefixes.SetPrefix(msg.Prefix, msg.Uri);
        }

        #endregion

        #region IRpcHandler implementation

        /// <summary>
        /// Calls a RPC procedure with provided params.
        /// </summary>
        /// <param name="callId">Client generated call identifier.</param>
        /// <param name="procId">URI procedure identfier.</param>
        /// <param name="args">Procedure invokation parameters.</param>
        /// <returns></returns>
        public object Call(string callId, string procId, object[] args)
        {
            if (this.procedures.ContainsKey(procId))
            {
                return this.procedures[procId](args);
            }
            else
            {
                throw new KeyNotFoundException(
                    "WAMP listener couldn't recognize procedure URI for invoked call request. Procedure URI: " + procId);
            }
        }

        /// <summary>
        /// Registers new method handler for target RPC procedure URI.
        /// </summary>
        /// <param name="procId"></param>
        /// <param name="func"></param>
        public void RegisterRpcAction(string procId, Func<object[], object> func)
        {
            if (this.procedures.ContainsKey(procId))
                this.procedures[procId] = func;
            else
                this.procedures.Add(procId, func);
        }

        /// <summary>
        /// Unregisters RPC method handler recognized by it's procedure URI.
        /// </summary>
        /// <param name="procId"></param>
        public void UnregisterRpcAction(string procId)
        {
            if (this.procedures.ContainsKey(procId))
                this.procedures.Remove(procId);
        }

        #endregion

        #region IWampSubscriber implementation

        /// <summary>
        /// Creates new topic with provided id. 
        /// This method works only when <see cref="FixedTopics"/> flag is set on True.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>True if new topic has been created, false otherwise.</returns>
        public bool CreateTopic(string topicId)
        {
            if (!this.topics.ContainsKey(topicId) && this.FixedTopics)
            {
                this.topics.Add(topicId, new HashSet<string>());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes existing topic with provided id. 
        /// This method works only when <see cref="FixedTopics"/> flag is set on True.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>True if topic has been removed successfully, false otherwise.</returns>
        public bool RemoveTopic(string topicId)
        {
            if (this.topics.ContainsKey(topicId) && this.FixedTopics)
            {
                this.topics[topicId].Clear();
                this.topics.Remove(topicId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Subscribes new WAMP connection to target topic. 
        /// If topic didn't exist before, then it'd be created.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="connection"></param>
        public void Subscribe(string topicId, IWampConnection connection)
        {
            if (this.topics.ContainsKey(topicId))
                this.topics[topicId].Add(connection.SessionId);
            else if(!this.FixedTopics)
                this.topics.Add(topicId, new HashSet<string>(new[] {connection.SessionId}));
        }

        /// <summary>
        /// Unsubscribes a WAMP connection from target topic. 
        /// It also removes topic if none connections participate in it.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="connection"></param>
        public void Unsubscribe(string topicId, IWampConnection connection)
        {
            if (this.topics.ContainsKey(topicId))
            {
                this.topics[topicId].Remove(connection.SessionId);
                if (this.topics[topicId].Count == 0 && !this.FixedTopics)
                    this.topics.Remove(topicId);
            }
        }

        /// <summary>
        /// Publishes new event object among specific subscribers assigned to target topic.
        /// </summary>
        /// <param name="topicId">Topic URI identifier.</param>
        /// <param name="senderSession">Event sender session identifier.</param>
        /// <param name="evt">Event object.</param>
        /// <param name="eligible">
        /// List of eligible receiver's sessions ids. In not null or empty,
        /// list of excludes and excludeMe flag will be ignored.
        /// </param>
        /// <param name="excludes">
        /// List of subscribers sessions ids, which should be excluded from message receival.
        /// </param>
        /// <param name="excludeMe">Should sender be excluded as message receiver?</param>
        public void Publish(string topicId, string senderSession, object evt, IEnumerable<string> eligible, IEnumerable<string> excludes, bool excludeMe)
        {
            var msg = new EventMessage(topicId, evt);
            var json = this.Serializer.SerializeArray(msg.ToArray());
            var receivers = GetEventReceivers(this.topics[topicId], senderSession, eligible, excludes, excludeMe);

            Parallel.ForEach(receivers, receiver =>
            {
                IWampConnection connection;
                if(this.connections.TryGetValue(receiver, out connection))
                {
                    connection.SendMessage(json);
                }
            });

            if(this.EventPublished != null)
                this.EventPublished(this, new PubSubEventArgs(topicId, senderSession, receivers));
        }

        /// <summary>
        /// Gets list of session identifiers for subscribers, which should receive an event message.
        /// </summary>
        /// <param name="topicSubscribers">Default list of all topic subscribers.</param>
        /// <param name="senderSession">Event emitter (sender's session id).</param>
        /// <param name="eligible">
        /// List of eligible receivers. If any, then excludes and exclude me will be ignored.
        /// </param>
        /// <param name="excludes">List of excluded connections sessions ids.</param>
        /// <param name="excludeMe">Should sender be excluded as message receiver?</param>
        /// <returns>Final list of session ids of WAMP connections to receive message.</returns>
        private static IEnumerable<string> GetEventReceivers(IEnumerable<string> topicSubscribers, string senderSession, IEnumerable<string> eligible, IEnumerable<string> excludes, bool excludeMe)
        {
            var receivers = topicSubscribers;
            if (eligible != null && eligible.Any())
                return topicSubscribers.Intersect(eligible);

            if (excludes != null && excludes.Any())
                receivers = receivers.Except(excludes);
            if (excludeMe)
                receivers = receivers.Except(new[] {senderSession});
            return receivers;
        }

        #endregion

        public virtual void Dispose()
        {
        }
    }
}