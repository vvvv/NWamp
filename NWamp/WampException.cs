namespace NWamp
{
    using System;

    /// <summary>
    /// Common exception derived by all NWAMP custom implementation exceptions.
    /// </summary>
    public class WampException :Exception
    {
        public WampException(string msg):base(msg)
        {
        }
    }
}