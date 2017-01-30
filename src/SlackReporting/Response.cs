using Newtonsoft.Json;

namespace ITGlobal.SlackReporting
{
    internal abstract class Response
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}