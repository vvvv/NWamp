namespace NWamp.Protocol.Messages
{
    public interface IWampMessage
    {
        object[] ToArray();
        void FromArray(object[] array);
        MessageTypes Type { get; }
    }
}