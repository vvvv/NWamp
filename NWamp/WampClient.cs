using System.Collections.Generic;
using System.Linq;
using System.Net;
using NWamp.Mapping;
using NWamp.Transport;
using NWamp.Protocol.Messages;
using System;

namespace NWamp
{
    /// <summary>
    /// Client-side WAMP protocol implementation.
    /// </summary>
    public class WampClient : BaseWampProtocol
    {
        #region Fields & Properties


        /// <summary>
        /// Web socket access interface used for communication.
        /// </summary>
        public IWebSocket Socket { get; protected set; }

        /// <summary>
        /// List of current calls.
        /// </summary>
        private readonly IList<string> calls;

        /// <summary>
        /// List of currently subscribed topics.
        /// </summary>
        private readonly HashSet<string> topics;

        /// <summary>
        /// List of prefixes defined by current client.
        /// </summary>
        public PrefixMap Prefixes { get; private set; }

        /// <summary>
        /// Current client session identifier.
        /// </summary>
        public string SessionId { get; protected set; }

        /// <summary>
        /// Checks if current client is connected to WAMP listener.
        /// </summary>
        public bool IsConnected { get; protected set; }

        #endregion

        /// <summary>
        /// Creates new instance of <see cref="WampClient"/> class.
        /// </summary>
        /// <param name="socket"></param>
        public WampClient(IWebSocket socket)
            : this(socket, new DefaultJsonSerializer())
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="WampClient"/> class.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="serializer"></param>
        public WampClient(IWebSocket socket, IJsonSerializer serializer) : base(serializer)
        {
            this.Socket = socket;
            this.calls = new List<string>();
            this.topics = new HashSet<string>();
            this.Prefixes = new PrefixMap();
        }

        /// <summary>
        /// Connects to new WAMP listener, which is listening under provided url.
        /// </summary>
        /// <param name="uri"></param>
        public void Connect(string uri)
        {
            
        }

        #region WAMP client actions

        /// <summary>
        /// Sets new CURIE->URI mapping between current client and connected WAMP server.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="uri">Valid URI resource path.</param>
        public void Prefix(string prefix, string uri)
        {
            var msg = new PrefixMessage(prefix, uri);
            var json = this.SerializeMessage(msg);
            
            this.Socket.SendMessage(json);
        }

        /// <summary>
        /// Subscribes current WAMP client to target topic.
        /// </summary>
        /// <param name="topic"></param>
        public void Subscribe(string topic)
        {
            var subMsg = new SubscribeMessage(topic);
            var json = this.Serializer.SerializeArray(subMsg.ToArray());
            this.Socket.SendMessage(json);

            this.topics.Add(topic);
        }

        /// <summary>
        /// Unsubscribes current WAMP client from target topic.
        /// </summary>
        /// <param name="topic"></param>
        public void Unsubscribe(string topic)
        {
            var subMsg = new UnsubscribeMessage(topic);
            var json = this.SerializeMessage(subMsg);
            this.Socket.SendMessage(json);

            this.topics.Remove(topic);
        }

        /// <summary>
        /// Publishes new event object on target topic id.
        /// </summary>
        /// <param name="topicId">Topic identifier.</param>
        /// <param name="evt">Event object published among all topic subscribers.</param>
        /// <param name="excludeMe">Should current client receive publish message?</param>
        public void PublishAll(string topicId, object evt, bool excludeMe = false)
        {
            this.Publish(topicId, evt, excludeMe, null, null);
        }

        /// <summary>
        /// Publishes new event object on target topic id.
        /// </summary>
        /// <param name="topicId">Topic identifier.</param>
        /// <param name="evt">Event object published among eligible subscribers.</param>
        /// <param name="eligibles">List of eligible receivers.</param>
        public void PublishTo(string topicId, object evt, IEnumerable<string> eligibles)
        {
            this.Publish(topicId, evt, null, eligibles, null);
        }

        /// <summary>
        /// Publishes new event object on target topic id.
        /// </summary>
        /// <param name="topicId">Topic identifier.</param>
        /// <param name="evt">
        /// Event object published among all topic subscribers, except the specified ones.
        /// </param>
        /// <param name="excludes">List of excluded topic subscribers.</param>
        public void PublishExcept(string topicId, object evt, IEnumerable<string> excludes)
        {
            this.Publish(topicId, evt, null, null, excludes);
        }

        /// <summary>
        /// Performs a RPC action call with specified parameters.
        /// </summary>
        /// <param name="procUri"></param>
        /// <param name="args"></param>
        public void Call(string procUri, params object[] args)
        {
            var callId = this.GenerateSessionKey(16);
            var msg = new CallMessage(callId, procUri, args);
            var json = this.SerializeMessage(msg);

            this.Socket.SendMessage(json);
            this.calls.Add(callId);
        }

        private void Publish(string topicId, object evt, bool? excludeMe, IEnumerable<string> eligibles, IEnumerable<string> excludes)
        {
            var msg = excludeMe.HasValue
                ? new PublishMessage(topicId, evt, excludeMe.Value)
                : new PublishMessage(topicId, evt, eligibles.ToList(), excludes.ToList());

            var json = this.SerializeMessage(msg);
            this.Socket.SendMessage(json);
        }

        #endregion

        #region Message receiving & responding

        /// <summary>
        /// Method invoked when new WAMP message has been received.
        /// </summary>
        /// <param name="msg"></param>
        protected bool OnMessageReceived(IWampMessage msg)
        {
            var type = msg.Type;
            switch (type)
            {
                case MessageTypes.Welcome:
                    OnWelcomeReceived((WelcomeMessage)msg);
                    return true;

                case MessageTypes.CallResult:
                    OnCallResult((CallResultMessage)msg);
                    return true;

                case MessageTypes.CallError:
                    OnCallError((CallErrorMessage)msg);
                    return true;

                case MessageTypes.Event:
                    OnEvent((EventMessage)msg);
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method invoked, when new <see cref="EventMessage"/> has been received.
        /// </summary>
        /// <param name="eventMessage"></param>
        protected virtual void OnEvent(EventMessage eventMessage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Method invoked, when new <see cref="CallErrorMessage"/> has been received.
        /// </summary>
        /// <param name="callErrorMessage"></param>
        protected virtual void OnCallError(CallErrorMessage callErrorMessage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Method invoked, when new <see cref="CallResultMessage"/> has been received.
        /// </summary>
        /// <param name="callResultMessage"></param>
        protected virtual void OnCallResult(CallResultMessage callResultMessage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Method invoked when new WAMP welcome message has been received.
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnWelcomeReceived(WelcomeMessage msg)
        {
            if (WampConfiguration.ProtocolVersion != msg.ProtocolVersion)
                throw new ProtocolViolationException(
                    string.Format("Incompatybile WAMP protocol versions. Currently accepted version is {0}, but WELCOME version is {1}",
                    WampConfiguration.ProtocolVersion,
                    msg.ProtocolVersion));

            this.SessionId = msg.SessionId;
            this.IsConnected = true;
        }

        #endregion
    }
}