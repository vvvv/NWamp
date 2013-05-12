namespace NWamp
{
    using System;
    using NWamp.Protocol.Messages;

    /// <summary>
    /// Base class used by both, server- and client-side WAMP implementations.
    /// Don't derive from it, until it's realy needed. Instead use <see cref="WampListener"/>
    /// or <see cref="WampClient"/> classes.
    /// </summary>
    public abstract class BaseWampProtocol
    {
        private Random rand;

        /// <summary>
        /// Gets or sets delegate used for deserializing WAMP message frames.
        /// </summary>
        public Func<string, object> DeserializeMessageFrame { get; set; }

        /// <summary>
        /// Gets or sets delegate used for serializing WAMP message frames.
        /// </summary>
        public Func<object, string> SerializeMessageFrame { get; set; }

        protected BaseWampProtocol()
        {
            this.rand = new Random();
        }

        protected BaseWampProtocol(Func<object, string> serializer, Func<string, object> deserializer)
        {
            this.rand = new Random();
            this.DeserializeMessageFrame = deserializer;
            this.SerializeMessageFrame = serializer;
        }

        /// <summary>
        /// Creates new session id.
        /// </summary>
        /// <param name="size">Session key length.</param>
        protected string GenerateSessionKey(int size=16)
        {
            const string alphas =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_-";

            var chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = alphas[rand.Next(alphas.Length)];
            }
            return new string(chars);
        }

        /// <summary>
        /// Serializes provided WAMP message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected string SerializeMessage(IWampMessage message)
        {
            var json = this.SerializeMessageFrame(message.ToArray());
            return json;
        }
    }
}