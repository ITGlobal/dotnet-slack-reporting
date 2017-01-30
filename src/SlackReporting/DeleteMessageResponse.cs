using Newtonsoft.Json;

namespace ITGlobal.SlackReporting
{
    internal sealed class DeleteMessageResponse : Response
    {
        [JsonProperty("ts")]
        public string Timestamp { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }
    }
}