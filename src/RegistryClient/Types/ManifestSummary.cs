using Newtonsoft.Json;

namespace RegistryClient
{
    public class ManifestSummary
    {
        [JsonProperty("mediaType")]
        public string MediaType { get; set; }
        [JsonProperty("size")]
        public long Size { get; set; }
        [JsonProperty("digest")]
        public string Digest { get; set; }
        [JsonProperty("platform")]
        public Platform Platform { get; set; }
    }
}
