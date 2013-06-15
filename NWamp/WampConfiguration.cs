namespace NWamp
{
    using System;
    using System.Reflection;
    using System.Diagnostics;

    /// <summary>
    /// Basic configuration informations.
    /// </summary>
    public static class WampConfiguration
    {
        /// <summary>
        /// Version number of NWAMP library used. This should be visible to client connections.
        /// </summary>
        private static readonly Version version;

        static WampConfiguration()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fi = FileVersionInfo.GetVersionInfo(asm.Location);
            version = new Version(fi.ProductMajorPart, fi.ProductMinorPart, fi.ProductBuildPart);
        }

        /// <summary>
        /// Gets WAMP protocol implementation info.
        /// </summary>
        public static string Implementation
        {
            get
            {
                return "NWamp/" + version;
            }
        }

        /// <summary>
        /// Gets NWamp framework version (based on assembly).
        /// </summary>
        public static Version Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// Gets Implemented WAMP protocol version.
        /// </summary>
        public static double ProtocolVersion { get { return 1.0; } }
    }
}