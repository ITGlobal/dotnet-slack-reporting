using System;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     An event args for <see cref="SlackReporter.DiagnosticsMessage"/>
    /// </summary>
    public sealed class DiagnosticsMessageEventArgs : EventArgs
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        internal DiagnosticsMessageEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        ///     Message text
        /// </summary>
        public string Message { get; }
    }
}