using System.IO;
using System.Runtime.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWamp
{
    /// <summary>
    /// Default implementation of JSON serializer using native .NET library
    /// and <see cref="DataContractJsonSerializer"/> class.
    /// </summary>
    public class DefaultJsonSerializer :  IJsonSerializer
    {
        private static readonly List<Type> knownTypes = new List<Type>
        {
            typeof(int),
            typeof(string),
            typeof(bool),
            typeof(object[])
        };

        public string SerializeArray(object[] args)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = GetSerializer();
                serializer.WriteObject(stream, args);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public object[] DeserializeArray(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                var serializer = GetSerializer();
                return (object[])serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Add new type to list of types known to JSON serializer.
        /// </summary>
        public static void AddKnownType<TType>()
        {
            knownTypes.Add(typeof(TType));
        }

        /// <summary>
        /// Add new type to list of types known to JSON serializer.
        /// </summary>
        public static void AddKnownType(Type type)
        {
            knownTypes.Add(type);
        }

        /// <summary>
        /// Removes type from list of types known to JSON serializer.
        /// </summary>
        public static void RemoveKnownType<TType>()
        {
            knownTypes.Remove(typeof(TType));
        }

        /// <summary>
        /// Removes type from list of types known to JSON serializer.
        /// </summary>
        public static void RemoveKnownType(Type type)
        {
            knownTypes.Remove(type);
        }

        /// <summary>
        /// Gets new instance of <see cref="DataContractJsonSerializer"/> with set of known types.
        /// </summary>
        /// <returns></returns>
        private static DataContractJsonSerializer GetSerializer()
        {
            return new DataContractJsonSerializer(typeof (object[]), knownTypes);
        }
    }
}