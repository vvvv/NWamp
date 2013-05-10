namespace NWamp.Protocol.Rpc
{
    using System;

    /// <summary>
    /// Interface used for handling WAMP RPC connections. Defines functionalities required
    /// for handling WAMP RPC requests.
    /// </summary>
    public interface IRpcHandler
    {
        /// <summary>
        /// Performs RPC method call. This is a synchronous method.
        /// </summary>
        /// <param name="callId">Current call identifier.</param>
        /// <param name="procUri">Procedure URI identifier.</param>
        /// <param name="args">Call method arguments.</param>
        /// <returns>Value returned from method called.</returns>
        object Call(string callId, string procUri, params object[] args);

        /// <summary>
        /// Registers method handler delegate for WAMP procedure identifier by provided URI.
        /// </summary>
        /// <param name="procUri">Procedure URI identifier.</param>
        /// <param name="handler">Method handler to register.</param>
        void RegisterRpcAction(string procUri, Func<object[], object> handler);

        /// <summary>
        /// Removes method handler delegate from list of delegates registered for WAMP RPC procedures.
        /// </summary>
        /// <param name="procUri">Procedure URI identifier.</param>
        void UnregisterRpcAction(string procUri);

    }
}