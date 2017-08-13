using Newtonsoft.Json;

namespace Scottz0r.MarkdownSite.Models
{
    public class DtoWrapper
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorData Error { get; set; }
    }
}
