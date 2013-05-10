using System;
using NWamp.Transport;
using NWamp.Protocol.Messages;

namespace NWamp
{

    /// <summary>
    /// Base class used by both, server- and client-side WAMP implementations.
    /// </summary>
    public abstract class BaseWampProtocol
    {
        private Random rand;

        /// <summary>
        /// Serializer used for serializing WAMP messages into valid JSON objects and vice versa.
        /// </summary>
        public IJsonSerializer Serializer { get; set; }

        protected BaseWampProtocol()
            : this(new DefaultJsonSerializer())
        {
        }

        protected BaseWampProtocol(IJsonSerializer serializer)
        {
            this.Serializer = serializer;
            this.rand = new Random();
        }

        /// <summary>
        /// Creates new session id.
        /// </summary>
        /// <param name="size">Session key length.</param>
        /// <returns></returns>
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
            var json = this.Serializer.SerializeArray(message.ToArray());
            return json;
        }
    }
}