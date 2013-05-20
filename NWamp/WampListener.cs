namespace NWamp
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NWamp.Mapping;
    using NWamp.Protocol.Events;
    using NWamp.Protocol.Messages;
    using NWamp.Protocol.Rpc;
    using NWamp.Transport;
    using System.Threading;

    /// <summary>
    /// Server-side listener using WAMP protocol. Derive from this class to create working 
    /// Websocket Application Message Protocol server-side listener based on choosen web sockets implementation.
    /// </summary>
    public abstract class WampListener : BaseWampProtocol, IRpcHandler, IWampSubscriber, IDisposable
    {
        #region Properties & Fields

        /// <summary>
        /// Dictionary containing list of actualy connected clients recognized by their session ids.
        /// </summary>
        protected readonly IDictionary<string, IWampConnection> connections;

        /// <summary>
        /// Map of topics and their subscribers sessions hashes.
        /// </summary>
        protected readonly IDictionary<string, HashSet<string>> topics;

        /// <summary>
        /// Map of procedures associated with RPC action calls. Procedures are just simple
        /// .NET methods represented in form of the delegates. 
        /// </summary>
        /// <remarks>
        /// Key is procedure id used to recognize registered method handler from client-side perspective.
        /// 
        /// Func&lt;object[], object&gt; is a generalization. You may create specialized generic methods
        /// by using extension methods found in <see cref="RpcExtensions"/> class.
        /// </remarks>
        protected readonly IDictionary<string, Func<object[], object>> procedures;

        /// <summary>
        /// List of RPC calls realized at the moment. Each RPC call is realized asynchronously
        /// to asure, that current listener will not be blocked by the call.
        /// </summary>
        private readonly ConcurrentDictionary<string, Task> calls;

        /// <summary>
        /// Gets collection of RPC calls realized at the moment. Each RPC call is realized asynchronously
        /// to asure, that current listener will not be blocked by the call.
        /// 
        /// Keys are session hashes used to recognize requester's connections.
        /// </summary>
        public IDictionary<string, Task> CurrentCalls
        {
            get { return this.calls; }
        }

        /// <summary>
        /// Gets or sets value determining, if listener should block creating topics on client's demand.
        /// 
        /// If this property is set to true, client's could not create new topics on demand, they only
        /// can join to topics created before.
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
        /// Event raised, before new event will be published among topic subscribers.
        /// </summary>
        public event PublishingEventHandler EventPublishing;

        /// <summary>
        /// Event raised, after new event will be published among topic subscribers.
        /// </summary>
        public event PublishedEventHandler EventPublished;

        /// <summary>
        /// Event fired, when new topic has been created.
        /// </summary>
        public event TopicCreatedEventHandler TopicCreated;

        /// <summary>
        /// Event fired, before new topic will be created.
        /// </summary>
        public event TopicCreatingEventHandler TopicCreating;

        /// <summary>
        /// Event fired, after topic has been removed.
        /// </summary>
        public event TopicRemovedEventHandler TopicRemoved;

        /// <summary>
        /// Event fired, before new connection will be subscribed.
        /// </summary>
        public event ConnectionSubscribingEventHandler ConnectionSubscribing;

        /// <summary>
        /// Event fired, after new connection has been subscribed.
        /// </summary>
        public event ConnectionSubscribedEventHandler ConnectionSubscribed;

        /// <summary>
        /// Event fired, when existing connection has been unsubscribed.
        /// </summary>
        public event ConnectionUnsubscribedEventHandler ConnectionUnsubscribed;

        /// <summary>
        /// Event raised when new RPC call has been received and started to work.
        /// </summary>
        public event CallInvokingEventHandler CallInvoking;

        /// <summary>
        /// Event raised when existing RPC call has ended.
        /// </summary>
        public event CallInvokedEventHandler CallInvoked;

        #endregion

        /// <summary>
        /// Create new instance of <see cref="WampListener"/>.
        /// </summary>
        public WampListener()
            : base()
        {
            this.connections = new Dictionary<string, IWampConnection>();
            this.topics = new Dictionary<string, HashSet<string>>();
            this.procedures = new Dictionary<string, Func<object[], object>>();
            this.calls = new ConcurrentDictionary<string, Task>();
        }

        /// <summary>
        /// Create new instance of <see cref="WampListener"/>.
        /// </summary>
        /// <param name="serializer">Custom JSON serialization method.</param>
        /// <param name="deserializer">Custom JSON deserialization method.</param>
        public WampListener(Func<object[], string> serializer, Func<string, object[]> deserializer)
            : base(serializer, deserializer)
        {
            this.connections = new Dictionary<string, IWampConnection>();
            this.topics = new Dictionary<string, HashSet<string>>();
            this.procedures = new Dictionary<string, Func<object[], object>>();
            this.calls = new ConcurrentDictionary<string, Task>();
        }

        /// <summary>
        /// Method invoked when new connection has been established.
        /// 
        /// Creates new session key and sends welcome message frame.
        /// </summary>
        /// <param name="connection">Reference to web socket connection, allowing to push messages to clients.</param>
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
        /// Closes session removing session identifier.
        /// </summary>
        /// <param name="connection">Reference to disconnected or closed web socket connection.</param>
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
        /// String messages are just serialized JSON arrays - WAMP protocol frames.
        /// </summary>
        /// <param name="connection">Reference to web socket connection, which sent a message.</param>
        public virtual void OnReceived(string json, IWampConnection connection)
        {
            var array = this.DeserializeMessageFrame(json) as object[];
            if (array != null)
            {
                var msg = MessageMapper.Get(array);

                this.OnMessageReceived(msg, connection);
            }
            else
            {
                throw new MessageParsingException("Couldn't parse received WAMP message frame", array);
            }
        }

        #region Message responding

        /// <summary>
        /// Method invoked when new message has been received.
        /// This method is called from within <see cref="OnReceived"/> method when JSON string 
        /// has been deserialized successfully.
        /// 
        /// It calls specialized WAMP message frame handlers.
        /// </summary>
        /// <param name="message">Deserialized WAMP message frame.</param>
        /// <param name="connection">Reference to web socket connection, which sent a message.</param>
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
        /// Publishes message event through all subbscribers defined inside message frame.
        /// </summary>
        protected void OnPublishMessage(PublishMessage message, IWampConnection connection)
        {
            var topicId = connection.Prefixes.Map(message.TopicUri);
            this.Publish(topicId, connection.SessionId, message.Event, message.Eligibles, message.Excludes, message.ExcludeMe);
        }

        /// <summary>
        /// Method invoked when new <see cref="UnsubscribeMessage"/> has been received.
        /// Unsubscribes current <paramref name="connection"/> from topic sent with WAMP message frame.
        /// </summary>
        protected void OnUnsubscribeMessage(UnsubscribeMessage message, IWampConnection connection)
        {
            var topicUri = connection.Prefixes.Map(message.TopicUri);
            this.Unsubscribe(topicUri, connection);
        }

        /// <summary>
        /// Method invoked when new <see cref="SubscribeMessage"/> has been received.
        /// Subscribes current <paramref name="connection"/> to topic sent with WAMP message frame.
        /// </summary>
        protected void OnSubscribeMessage(SubscribeMessage message, IWampConnection connection)
        {
            var topicUri = connection.Prefixes.Map(message.TopicUri);
            this.Subscribe(topicUri, connection);
        }

        /// <summary>
        /// Method invoked when new <see cref="CallMessage"/> has been received.
        /// Begins asynchronous call of .NET method registered under 'call Id' found inside 
        /// WAMP message frame.
        /// </summary>
        protected void OnCallMessage(CallMessage message, IWampConnection connection)
        {
            var tokenSource = new CancellationTokenSource();
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
                    // try to invoke registered method and return call result message frame

                    var result = this.Call(callId, procUri, args.ToArray());
                    var callResponse = new CallResultMessage(callId, result);
                    var json = this.SerializeMessageFrame(callResponse.ToArray());
                    connection.SendMessage(json);
                }
                catch (CallErrorException exc)
                {
                    // we can react on inner RPC procedures exception if they are thrown as CallErrorExceptions
                    // in that case, send call error message frame

                    var errorUri = (conn.Prefixes.ContainsMapping(exc.ErrorUri))
                                       ? conn.Prefixes.Map(exc.ErrorUri)
                                       : exc.ErrorUri;

                    var callError = new CallErrorMessage(exc.CallId, errorUri, exc.Description, exc.Details);
                    var json = this.SerializeMessageFrame(callError.ToArray());
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
                    this.calls.TryRemove(callId, out t);    // try to remove method call handler

                    // fire event on call finished
                    if (this.CallInvoked != null)
                        this.CallInvoked(this,
                            new CallInvokedEventArgs(callId, procUri, conn.SessionId, exception));
                }
            }, new object[] { message.CallId, message.ProcUri, message.Arguments, connection }, tokenSource.Token);

            // try to add method call handler
            this.calls.TryAdd(message.CallId, callTask);

            // fire event on call started
            if (this.CallInvoking != null)
            {
                var invokingEvt = new CallInvokingEventArgs(message.CallId,
                        connection.Prefixes.Map(message.ProcUri), connection.SessionId);
                
                this.CallInvoking(this, invokingEvt);

                if (invokingEvt.Cancel)
                {
                    tokenSource.Cancel();
                }
            }
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
        /// Calls a RPC procedure with provided params. This is a synchronous (thread-blocking) operation.
        /// </summary>
        /// <param name="callId">Client generated call identifier.</param>
        /// <param name="procId">URI procedure identfier.</param>
        /// <param name="args">Procedure invokation parameters.</param>
        /// <returns>Result of method handler computations.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Exception thrown when no method handler has been registered under provided <paramref name="procId"/>.
        /// </exception>
        public object Call(string callId, string procId, object[] args)
        {
            Func<object[], object> func;
            if (this.procedures.TryGetValue(procId, out func))
            {
                return func(args);
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
        /// <param name="procId">Procedure identifier.</param>
        /// <param name="handler">Method handler.</param>
        public void RegisterRpcAction(string procId, Func<object[], object> handler)
        {
            if (this.procedures.ContainsKey(procId))
                this.procedures[procId] = handler;
            else
                this.procedures.Add(procId, handler);
        }

        /// <summary>
        /// Unregisters RPC method handler recognized by it's procedure URI.
        /// </summary>
        /// <param name="procId">Procedure identifier.</param>
        public void UnregisterRpcAction(string procId)
        {
            if (this.procedures.ContainsKey(procId))
                this.procedures.Remove(procId);
        }

        #endregion

        #region IWampSubscriber implementation

        /// <summary>
        /// Creates new topic with provided id. 
        /// This method works only when <see cref="FixedTopics"/> 
        /// flag is set on True or <paramref name="force"/> argument is set.
        /// </summary>
        /// <returns>True if new topic has been created, false otherwise.</returns>
        public virtual bool CreateTopic(string topicId, bool force = false)
        {
            if (!this.topics.ContainsKey(topicId) && (this.FixedTopics || force))
            {
                var evt = new TopicCreatingEventArgs(topicId);
                if (this.TopicCreating != null)
                    this.TopicCreating(this, evt);

                if (!evt.Cancel)
                {
                    this.topics.Add(topicId, new HashSet<string>());

                    if (this.TopicCreated != null)
                        this.TopicCreated(this, new TopicCreatedEventArgs(topicId));

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes existing topic with provided id. 
        /// This method works only when <see cref="FixedTopics"/> 
        /// flag is set on True or <paramref name="force"/> argument is set.
        /// </summary>
        /// <returns>True if topic has been removed successfully, false otherwise.</returns>
        public virtual bool RemoveTopic(string topicId, bool force = false)
        {
            if (this.topics.ContainsKey(topicId) && (this.FixedTopics || force))
            {
                this.topics[topicId].Clear();
                this.topics.Remove(topicId);

                if (this.TopicRemoved != null)
                    this.TopicRemoved(this, new TopicRemovedEventArgs(topicId));

                return true;
            }
            return false;
        }

        /// <summary>
        /// Subscribes new WAMP connection to target topic. 
        /// If topic didn't exist before, then it'd be created.
        /// </summary>
        public void Subscribe(string topicId, IWampConnection connection)
        {
            HashSet<string> subscribers;
            if (this.topics.TryGetValue(topicId, out subscribers) || !this.FixedTopics)
            {
                if (subscribers == null)
                {
                    subscribers = new HashSet<string>();
                    this.topics.Add(topicId, subscribers);
                }

                var evt = new ConnectionSubscribingEventArgs(topicId, connection.SessionId);
                if (this.ConnectionSubscribing != null)
                    this.ConnectionSubscribing(this, evt);

                if (!evt.Cancel)
                {
                    subscribers.Add(connection.SessionId);

                    if (this.ConnectionSubscribed != null)
                        this.ConnectionSubscribed(this, new ConnectionSubscribedEventArgs(topicId, connection.SessionId));
                }
            }
        }

        /// <summary>
        /// Unsubscribes a WAMP connection from target topic. 
        /// It also removes topic if none connections participate in it.
        /// </summary>
        public void Unsubscribe(string topicId, IWampConnection connection)
        {
            if (this.topics.ContainsKey(topicId))
            {
                this.topics[topicId].Remove(connection.SessionId);
                if (this.topics[topicId].Count == 0 && !this.FixedTopics)
                {
                    this.topics.Remove(topicId);

                    if (this.ConnectionUnsubscribed != null)
                        this.ConnectionUnsubscribed(this, new ConnectionUnsubscribedEventArgs(topicId, connection.SessionId));
                }
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
            var json = this.SerializeMessageFrame(msg.ToArray());
            var receivers = GetEventReceivers(this.topics[topicId], senderSession, eligible, excludes, excludeMe);

            var publishingEvt = new PublishingEventArgs(topicId, senderSession, receivers);
            if (this.EventPublishing != null)
            {
                this.EventPublishing(this, publishingEvt);
                receivers = publishingEvt.ReceiversSessions;
            }

            var succeeded = new ConcurrentQueue<string>();
            Parallel.ForEach(receivers, receiver =>
            {
                IWampConnection connection;
                if (this.connections.TryGetValue(receiver, out connection))
                {
                    connection.SendMessage(json);
                    succeeded.Enqueue(receiver);
                }
            });

            if (this.EventPublished != null)
                this.EventPublished(this, new PublishedEventArgs(topicId, senderSession, succeeded));
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
                receivers = receivers.Except(new[] { senderSession });
            return receivers;
        }

        #endregion

        public abstract void Listen();

        public virtual void Dispose()
        {
        }
    }
}