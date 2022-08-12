// <copyright file="CodeExampleText.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CodeExampleText
    {
        [JsonProperty("ExampleNumber")]
        public int ExampleNumber { get; set; }

        [JsonProperty("ExampleName")]
        public string ExampleName { get; set; }

        [JsonProperty("ExampleDescription")]
        public string ExampleDescription { get; set; }

        [JsonProperty("ExampleDescriptionExtended")]
        public string ExampleDescriptionExtended { get; set; }

        [JsonProperty("PageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("ResultsPageHeader")]
        public string ResultsPageHeader { get; set; }

        [JsonProperty("ResultsPageText")]
        public string ResultsPageText { get; set; }

        [JsonProperty("SkipForLanguages")]
        public string SkipForLanguages { get; set; }

        [JsonProperty("LinksToAPIMethod")]
        public List<LinkToAPIMethods> LinksToAPIMethod { get; } = new List<LinkToAPIMethods>();

        [JsonProperty("AdditionalPage")]
        public List<AdditionalPage> AdditionalPages { get; } = new List<AdditionalPage>();
    }
}
