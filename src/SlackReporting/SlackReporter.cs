using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     Slack message reported
    /// </summary>
    public sealed class SlackReporter
    {
        private readonly string _baseUrl;

        internal SlackReporter(string baseUrl, SlackReporterOptions options)
        {
            _baseUrl = baseUrl;
            Options = options;

            var r = Call<AuthTestResponse>("auth.test", new Dictionary<string, string>());
            PrintDiagnosticsMessage($"Logged in as [{r.UserId}] '{r.User}' from [{r.TeamId}] '{r.Team}'");
        }

        internal SlackReporterOptions Options { get; }

        /// <summary>
        ///     A hook event to print diagnostics messages
        /// </summary>
        public static event EventHandler<DiagnosticsMessageEventArgs> DiagnosticsMessage;

        internal static void PrintDiagnosticsMessage(string message)
        {
            var handler = DiagnosticsMessage;
            if (handler != null)
            {
                handler(null, new DiagnosticsMessageEventArgs(message));
            }
        }

        /// <summary>
        ///     Begins new updatable message
        /// </summary>
        public SlackMessage BeginMessage(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return new SlackMessage(this, text);
        }

        internal T Call<T>(string method, IDictionary<string, string> args)
            where T : Response
        {
            using (var http = new HttpClient())
            {
                args["token"] = Options.AccessToken;
                var url = _baseUrl + method;
                var result = http.PostAsync(url, new FormUrlEncodedContent(args)).Result;
                PrintDiagnosticsMessage($"POST {url} -> {result.StatusCode:D} {result.ReasonPhrase}");

                var str = result.Content.ReadAsStringAsync().Result;
                var r = JsonConvert.DeserializeObject<T>(str);

                if (!r.Ok)
                {
                    PrintDiagnosticsMessage($"{method} -> {r.Error}");
                    throw new SlackReporterException(r.Error, $"An error while calling {method}: [{r.Error}]");
                }

                PrintDiagnosticsMessage($"{method} -> ok");
                return r;
            }
        }
    }
}