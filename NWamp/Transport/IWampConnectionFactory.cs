namespace NWamp.Transport
{
    /// <summary>
    /// Connection factory used for instantiate WAMP connections.
    /// </summary>
    public interface IWampConnectionFactory
    {
        IWampConnection Create(params object[] args);
    }
}