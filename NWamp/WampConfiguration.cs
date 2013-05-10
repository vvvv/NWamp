using System;
using System.Reflection;
using System.Diagnostics;
namespace NWamp
{
    /// <summary>
    /// Basic configuration informations.
    /// </summary>
    public static class WampConfiguration
    {
        private static readonly Version version;
        static WampConfiguration()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fi = FileVersionInfo.GetVersionInfo(asm.Location);
            version = new Version(fi.ProductMajorPart, fi.ProductMinorPart, fi.ProductBuildPart);
        }

        /// <summary>
        /// WAMP protocol implementation info.
        /// </summary>
        public static string Implementation
        {
            get
            {
                return "NWamp/" + version;
            }
        }

        /// <summary>
        /// NWamp framework version (based on assembly).
        /// </summary>
        public static Version Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// Implemented WAMP protocol version.
        /// </summary>
        public static int ProtocolVersion { get { return 1; } }
    }
}