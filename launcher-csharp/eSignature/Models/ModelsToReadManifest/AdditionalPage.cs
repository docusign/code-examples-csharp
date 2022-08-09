using Newtonsoft.Json;

namespace DocuSign.CodeExamples.eSignature.Models
{
    public class AdditionalPage
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ResultsPageHeader")]
        public string ResultsPageHeader { get; set; }

        [JsonProperty("ResultsPageText")]
        public string ResultsPageText { get; set; }
    }
}
