using System;
using System.Collections.Generic;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     A single updatable Slack message
    /// </summary>
    public sealed class SlackMessage : IDisposable
    {
        private readonly SlackReporter _reporter;
        private readonly object _lock = new object();

        private bool _isDisposed;
        private string _ts;
        private string _channel;

        internal SlackMessage(SlackReporter reporter, string text)
        {
            _reporter = reporter;

            PostMessage(text);
        }

        /// <summary>
        ///     Updates a message text
        /// </summary>
        public void Update(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (_ts != null && _channel != null)
                {
                    try
                    {
                        UpdateMessage(text);
                    }
                    catch (SlackReporterException e)
                    {
                        switch (e.ErrorCode)
                        {
                            case "message_not_found":
                            case "edit_window_closed":
                                SlackReporter.PrintDiagnosticsMessage($"Unable to update a message: [{e.ErrorCode}]. Will post new message instead");
                                PostMessage(text);
                                break;
                        }
                    }

                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (_ts != null && _channel != null)
                {
                    if (_reporter.Options.DeleteMessageAfterwards)
                    {
                        DeleteMessage();
                    }
                }

                _isDisposed = true;
            }
        }

        private void PostMessage(string text)
        {
            var args = new Dictionary<string, string>
            {
                { "text", text},
                { "channel",_reporter.Options.Channel}
            };
            if (!string.IsNullOrEmpty(_reporter.Options.Username))
            {
                args["username"] = _reporter.Options.Username;
            }
            _reporter.Options.Icon?.SetIcon(args);

            SlackReporter.PrintDiagnosticsMessage($"Posting a message to '{_reporter.Options.Channel}'...");
            var r = _reporter.Call<PostMessageResponse>("chat.postMessage", args);

            _ts = r.Timestamp;
            _channel = r.Channel;
            SlackReporter.PrintDiagnosticsMessage($"A message has been posted: {{ ts = '{_ts}', channel = '{_channel}' }}");
        }

        private void UpdateMessage(string text)
        {
            var args = new Dictionary<string, string>
            {
                { "text", text},
                { "ts",_ts},
                { "channel",_channel}
            };
            if (!string.IsNullOrEmpty(_reporter.Options.Username))
            {
                args["username"] = _reporter.Options.Username;
            }
            _reporter.Options.Icon?.SetIcon(args);

            SlackReporter.PrintDiagnosticsMessage($"Updating a message {{ ts = '{_ts}', channel = '{_channel}' }}...");
            var r = _reporter.Call<UpdateMessageResponse>("chat.update", args);

            _ts = r.Timestamp;
            _channel = r.Channel;
            SlackReporter.PrintDiagnosticsMessage($"A message has been updated: {{ ts = '{_ts}', channel = '{_channel}' }}");
        }

        private void DeleteMessage()
        {
            var args = new Dictionary<string, string>
            {
                { "ts",_ts},
                { "channel",_channel}
            };
            if (!string.IsNullOrEmpty(_reporter.Options.Username))
            {
                args["username"] = _reporter.Options.Username;
            }
            _reporter.Options.Icon?.SetIcon(args);

            SlackReporter.PrintDiagnosticsMessage($"Deleting message {{ ts = '{_ts}', channel = '{_channel}' }}...");
            var r = _reporter.Call<DeleteMessageResponse>("chat.delete", args);

            _ts = r.Timestamp;
            _channel = r.Channel;
            SlackReporter.PrintDiagnosticsMessage($"A message has been deleted: {{ ts = '{_ts}', channel = '{_channel}' }}");
        }
    }
}