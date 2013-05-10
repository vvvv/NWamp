using System;

namespace NWamp
{
    /// <summary>
    /// Event arguments associated with target session event.
    /// </summary>
    public class SessionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets WAMP session identifier.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="SessionEventArgs"/>.
        /// </summary>
        /// <param name="sessionId"></param>
        public SessionEventArgs(string sessionId)
        {
            this.SessionId = sessionId;
        }
    }

    /// <summary>
    /// Delegate used for session events handling.
    /// </summary>
    /// <param name="sender">Event emitter.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void SessionEventHandler(object sender, SessionEventArgs e);
}