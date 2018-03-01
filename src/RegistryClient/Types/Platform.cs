using Newtonsoft.Json;

namespace RegistryClient
{
    public class Platform
    {
        [JsonProperty("architecture")]
        public string Architecture { get; set; }
        [JsonProperty("os")]
        public string Os { get; set; }
        [JsonProperty("os.version")]
        public string OsVersion { get; set; }
        [JsonProperty("os.features")]
        public string[] OsFeatures { get; set; }
        [JsonProperty("features")]
        public string[] Features { get; set; }
    }
}
