using System;
using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     Slack reporter options
    /// </summary>
    public sealed class SlackReporterOptions
    {
        private const string DEFAULT_URL = "https://slack.com/";
        private const string DEFAULT_CHANNEL = "#general";

        /// <summary>
        ///     Slack API root URL (e.g. https://slack.com/)
        ///     This is a required property.
        /// </summary>
        public string Url { get; set; } = DEFAULT_URL;

        /// <summary>
        ///     Slack access token.
        ///     This is a required property.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Slack channel name (with leading '#') or user name (with leading '@').
        ///     Messages will be sent to specified channel/user.
        ///     This is a required property.
        /// </summary>
        public string Channel { get; set; } = DEFAULT_CHANNEL;

        /// <summary>
        ///     Bot user name. 
        ///     This is an optional property.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Bot icon. 
        ///     This is an optional property.
        /// </summary>
        public SlackIcon Icon { get; set; }

        /// <summary>
        ///     'true' means that a message will be deleted when a <see cref="SlackMessage"/> is disposed.
        ///     This is an optional property.
        /// </summary>
        public bool DeleteMessageAfterwards { get; set; } = true;

        /// <summary>
        ///     Initializes a new instance of <see cref="SlackReporter"/>
        /// </summary>
        public SlackReporter CreateReporter()
        {
            Validate();

            var baseUrl = Url;
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            baseUrl += "api/";
            var reporter = new SlackReporter(baseUrl, this);

            return reporter;
        }

        private void Validate()
        {
            var errors = ValidateIterator().ToArray();
            if (errors.Length > 0)
            {
                throw new Exception(string.Join("; ", errors));
            }
        }

        private IEnumerable<string> ValidateIterator()
        {
            if (string.IsNullOrEmpty(Url))
            {
                yield return $"Property {nameof(Url)} is not set";
            }

            if (string.IsNullOrEmpty(AccessToken))
            {
                yield return $"Property {nameof(AccessToken)} is not set";
            }

            if (string.IsNullOrEmpty(Channel))
            {
                yield return $"Property {nameof(Channel)} is not set";
            }
        }
    }
}