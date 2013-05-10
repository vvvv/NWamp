using System;

namespace NWamp
{
    public interface IJsonSerializer
    {
        string SerializeArray(object[] args);
        object[] DeserializeArray(string json);
    }
}