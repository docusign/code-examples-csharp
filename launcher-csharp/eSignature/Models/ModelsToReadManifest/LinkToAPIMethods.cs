using Newtonsoft.Json;

namespace DocuSign.CodeExamples.eSignature.Models
{
    public class LinkToAPIMethods
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("PathName")]
        public string PathName { get; set; }
    }
}
