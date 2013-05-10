using NWamp.Protocol.Messages;
using System.Collections.Generic;
using System;

namespace NWamp.Mapping
{
    /// <summary>
    /// Class used to map JSON arrays into WAMP messages.
    /// </summary>
    public class MessageMapper
    {
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
            var a = (int)array[0];
            var type = (MessageTypes) a;
            return dictionary.ContainsKey(type)
                       ? dictionary[type](array)
                       : default(IWampMessage);
        }
    }
}