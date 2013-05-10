namespace NWamp
{
    /// <summary>
    /// Interface used to provide abstraction layer for possible JSON serialization frameworks.
    /// </summary>
    /// <remarks>
    /// Since all WAMP message frames are represented in form of JSON array collections,
    /// only this type of serialization is needed.
    /// 
    /// However, inner message frame objects may contain complex JSON objects, so full JSON support is required.
    /// </remarks>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes WAMP message frame (visible as multitype array) to JSON format.
        /// </summary>
        string SerializeArray(object[] args);

        /// <summary>
        /// Deserializes JSON string to form of array - representative of WAMP message frame.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        object[] DeserializeArray(string json);
    }
}