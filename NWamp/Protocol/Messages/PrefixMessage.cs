using System;

namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// Defines CURIE's prefix for specific Uris.
    /// </summary>
    public struct PrefixMessage : IWampMessage
    {
        /// <summary>
        /// Prefix of the CURIE notation.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Uri associated with target prefix.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PrefixMessage"/> class.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="uri"></param>
        public PrefixMessage(string prefix, string uri) : this()
        {
            if (!System.Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Provided URI is invalid: " + uri);
            }

            this.Prefix = prefix;
            this.Uri = uri;
        }

        /// <summary>
        /// Creates new instance of <see cref="PrefixMessage"/> based
        /// on previously parsed json array.
        /// </summary>
        /// <param name="array"></param>
        public PrefixMessage(object[] array) : this()
        {
            this.FromArray(array);
        }

        /// <summary>
        /// Maps current message into object array ready to parse by json serializer.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            return new object[]
                   {
                       (int)this.Type,
                       this.Prefix,
                       this.Uri
                   };
        }

        /// <summary>
        /// Parses provided WAMP frame to fill current instance with valid values.
        /// </summary>
        /// <param name="array"></param>
        public void FromArray(object[] array)
        {
            if (array.Length < 3)
                throw new MessageParsingException("Wrong number of array arguments", array);
            if (this.Type != (MessageTypes)(int)array[0])
                throw new MessageParsingException("Invalid array message type argument passed", array, this.Type);

            this.Prefix = (string)array[1];
            this.Uri = (string)array[2];
        }

        /// <summary>
        /// Type of WAMP message frame: 1.
        /// </summary>
        public MessageTypes Type
        {
            get { return MessageTypes.Prefix; }
        }
    }
}