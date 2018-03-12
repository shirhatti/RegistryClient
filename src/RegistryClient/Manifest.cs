using Newtonsoft.Json;

namespace RegistryClient
{
    public class Manifest
    {
        [JsonProperty("schemaVersion")]
        public long SchemaVersion { get; set; }
        [JsonProperty("mediaType")]
        public string MediaType { get; set; }
        [JsonProperty("config")]
        public Config Config { get; set; }
        [JsonProperty("layers")]
        public Config[] Layers { get; set; }
    }
}
