namespace NWamp.Protocol.Messages
{
    /// <summary>
    /// WAMP available frame/message types.
    /// </summary>
    public enum MessageTypes
    {
        Welcome     = 0,
        Prefix      = 1,
        Call        = 2,
        CallResult  = 3,
        CallError   = 4,
        Subscribe   = 5,
        Unsubscribe = 6,
        Publish     = 7,
        Event       = 8
    }
}