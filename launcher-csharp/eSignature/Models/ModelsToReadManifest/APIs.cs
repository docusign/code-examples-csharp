using Newtonsoft.Json;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.ESignature.Models
{
    public class APIs
    {
        [JsonProperty("Groups")]
        public List<ManifestGroup> Groups { get; } = new List<ManifestGroup>();

        [JsonProperty("Name")]
        public string Name { get; set; }
}
}
