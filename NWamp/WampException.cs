using System;
namespace NWamp
{
    public class WampException :Exception
    {
        public WampException(string msg):base(msg)
        {
            
        }
    }
}