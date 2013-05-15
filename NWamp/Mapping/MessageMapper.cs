namespace NWamp.Mapping
{
    using NWamp.Protocol.Messages;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// Class used to map JSON arrays into WAMP messages.
    /// </summary>
    public class MessageMapper
    {
        /// <summary>
        /// Factory-typed dictionary used to map WAMP serialized JSON into specific message frame objects.
        /// </summary>
        private static readonly IDictionary<MessageTypes, Func<object[], IWampMessage>> dictionary =
            new Dictionary <MessageTypes, Func<object[], IWampMessage>>
            {
                {MessageTypes.Welcome, (array) => new WelcomeMessage(array)},
                {MessageTypes.Prefix, (array) => new PrefixMessage(array)},
                {MessageTypes.Publish, (array) => new PublishMessage(array)},
                {MessageTypes.Subscribe, (array) => new SubscribeMessage(array)},
                {MessageTypes.Unsubscribe, (array) => new UnsubscribeMessage(array)},
                {MessageTypes.Event, (array) => new EventMessage(array)},
                {MessageTypes.Call, (array) => new CallMessage(array)},
                {MessageTypes.CallError, (array) => new CallErrorMessage(array)},
                {MessageTypes.CallResult, (array) => new CallResultMessage(array)}
            };

        public static IWampMessage Get(object[] array)
        {
            array[0] = Convert.ToInt32(array[0]);
            var type = (MessageTypes)array[0];
            Func<object[], IWampMessage> factoryMethod;

            return dictionary.TryGetValue(type, out factoryMethod)
                       ? factoryMethod(array)
                       : default(IWampMessage);
        }
    }
}