using DocuSign.CodeExamples.eSignature.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.eSignature.Models
{
    public class ManifestGroup
    {
        [JsonProperty("Examples")]
        public List<CodeExampleText> Examples { get; } = new List<CodeExampleText>();

        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
