using System.Collections.Specialized;
using System.Collections.Generic;
using System;

namespace NWamp.Mapping
{
    /// <summary>
    /// Class used to store prefix (CURIE->URI) mappings.
    /// </summary>
    public class PrefixMap
    {
        private readonly StringDictionary mappings;

        private string _defaultUri;

        /// <summary>
        /// Default uri returned when no requested prefix match.
        /// Available only when FaultTolerant is set to true.
        /// </summary>
        public string DefaultUri
        {
            get { return _defaultUri; }
            set
            {
                if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                    throw new UriFormatException("Uri parameter is not well formed uri string");

                _defaultUri = value;
            }
        }

        /// <summary>
        /// Flag determining behavior for non prefix matches.
        /// When set to true, DefaultUri will be returned.
        /// When set to false, exception will be thrown.
        /// </summary>
        public bool FaultTolerant { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PrefixMap"/>.
        /// </summary>
        public PrefixMap()
        {
            this.mappings = new StringDictionary();   
        }

        /// <summary>
        /// Creates new instance of <see cref="PrefixMap"/>.
        /// </summary>
        /// <param name="mappings"></param>
        public PrefixMap(IEnumerable<KeyValuePair<string, string>> mappings)
        {
            this.mappings = new StringDictionary();
            
            if(mappings != null)
                foreach (var mapping in mappings)
                {
                    this.SetPrefix(mapping.Key, mapping.Value);
                }
        }

        /// <summary>
        /// Creates new instance of <see cref="PrefixMap"/>.
        /// </summary>
        public PrefixMap(IEnumerable<KeyValuePair<string, string>> mappings, string defaulUri)
            : this(mappings)
        {
            if (!Uri.IsWellFormedUriString(defaulUri, UriKind.RelativeOrAbsolute))
                throw new UriFormatException("Uri parameter is not well formed uri string");

            this.FaultTolerant = true;
            this.DefaultUri = defaulUri;
        }

        /// <summary>
        /// Set new URI for provided prefix value.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="uri">String in URI format.</param>
        /// <returns>
        /// True if new mapping has been created.
        /// False if existing prefix has been overrided.
        /// </returns>
        public bool SetPrefix(string prefix, string uri)
        {
            if(!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                throw new UriFormatException("Uri parameter is not well formed uri string");

            if (this.mappings.ContainsKey(prefix))
            {
                this.mappings[prefix] = uri;
                return false;
            }
            this.mappings.Add(prefix, uri);
            return true;
        }

        /// <summary>
        /// Removes mapping associated with target prefix.
        /// </summary>
        public void RemovePrefix(string prefixOrUri)
        {
            if (!Uri.IsWellFormedUriString(prefixOrUri, UriKind.RelativeOrAbsolute))
            {
                var keyToRemove = string.Empty;
                foreach (KeyValuePair<string, string> mapping in this.mappings)
                {
                    if (mapping.Value == prefixOrUri)
                        keyToRemove = mapping.Key;
                }
                if (!string.IsNullOrEmpty(keyToRemove))
                    this.mappings.Remove(keyToRemove);
            }
            else
            {
                this.mappings.Remove(prefixOrUri);
            }
        }

        /// <summary>
        /// Maps given value into Uri available inside the mappings.
        /// </summary>
        public string Map(string prefixOrUri)
        {
            if (this.mappings.ContainsKey(prefixOrUri))
                return this.mappings[prefixOrUri];

            if (Uri.IsWellFormedUriString(prefixOrUri, UriKind.RelativeOrAbsolute))
                return prefixOrUri;

            if (this.FaultTolerant)
                return this.DefaultUri;
            
            throw new WampPrefixException("No matching prefix has been found.");
        }

        /// <summary>
        /// Checks if target prefix mapping exists inside the mapper.
        /// This method will not check if direct Uri mapping exists.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public bool ContainsPrefix(string prefix)
        {
            return this.mappings.ContainsKey(prefix);
        }

        /// <summary>
        /// Checks if target mapping exists inside the mapper.
        /// </summary>
        public bool ContainsMapping(string prefixOrUri)
        {
            if (Uri.IsWellFormedUriString(prefixOrUri, UriKind.RelativeOrAbsolute))
                return true;

            return this.mappings.ContainsKey(prefixOrUri);
        }
    }
}