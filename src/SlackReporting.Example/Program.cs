using System;
using System.Threading.Tasks;
using static System.Console;

namespace ITGlobal.SlackReporting
{
    public static class Program
    {
        private const int MAX_STEPS = 10;
        private const int DELAY_MS = 10 * 1000;

        private static readonly string[] OPERATIONS =
        {
            "Checking out sources...",
            "Cleaning temporary directories...",
            "Compiling project...",
            "*Warning* There are some compilation warnings",
            ":exclamation: Test _my-sample-test_ is taking too long",
            ":arrow_forward: Test run results:\n> Test 1/3 : OK\n> Test 2/3 : OK\n> Test 3/3 : OK",
        };

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                WriteLine("Invalid command line args!");
                WriteLine("Usage:");
                WriteLine("program.exe ACCESS_TOKEN CHANNEL_OR_USERNAME");
                Environment.Exit(-1);
                return;
            }

            var options = new SlackReporterOptions
            {
                AccessToken = args[0],
                Channel = args[1],
                Username = "Test bot",
                Icon = SlackIcon.Emoji(":information_source:"),
                DeleteMessageAfterwards = true
            };

            SlackReporter.DiagnosticsMessage += (_, e) => WriteLine($"[diag] {e.Message}");

            var reporter = options.CreateReporter();

            WriteLine("Posting initial status message");
            using (var message = reporter.BeginMessage("`[          ]` Preparing some long-term operation..."))
            {
                for (var i = 0; i < MAX_STEPS; i++)
                {
                    Task.Delay(MAX_STEPS).Wait();

                    var text = "`[";

                    var j = 0;
                    for (; j < i; j++)
                    {
                        text += '#';
                    }
                    for (; j < MAX_STEPS; j++)
                    {
                        text += ' ';
                    }

                    text += "]` ";
                    text += OPERATIONS[i % OPERATIONS.Length];

                    WriteLine($"Updating status message ({i + 1}/{MAX_STEPS})");
                    message.Update(text);
                }

                Task.Delay(MAX_STEPS).Wait();
                WriteLine("Deleting status message");
            }

            WriteLine("Done!");
        }
    }
}
