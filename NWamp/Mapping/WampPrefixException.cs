namespace NWamp.Mapping
{
    /// <summary>
    /// Exception used to signalize WAMP prefix or CURIE's errors.
    /// </summary>
    public class WampPrefixException : WampException
    {
        public WampPrefixException(string msg):base(msg){}
    }
}