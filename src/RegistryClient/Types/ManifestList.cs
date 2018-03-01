using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegistryClient
{
    public class ManifestList
    {
        [JsonProperty("schemaVersion")]
        public long SchemaVersion { get; set; }
        [JsonProperty("mediaType")]
        public string MediaType { get; set; }
        [JsonProperty("manifests")]
        public ManifestSummary[] Manifests { get; set; }
    }
}
