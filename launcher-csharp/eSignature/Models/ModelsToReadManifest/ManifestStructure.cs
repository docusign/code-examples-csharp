using Newtonsoft.Json;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.eSignature.Models
{
    public class ManifestStructure
    {
        [JsonProperty("Groups")]
        public List<ManifestGroup> Groups { get; } = new List<ManifestGroup>();
    }
}
