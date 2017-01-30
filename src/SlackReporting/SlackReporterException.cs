using System;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     An exception that occurs while calling Slack API
    /// </summary>
    public class SlackReporterException : Exception
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public SlackReporterException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        public SlackReporterException(string errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Error code from Slack API
        /// </summary>
        public string ErrorCode { get; }
    }
}