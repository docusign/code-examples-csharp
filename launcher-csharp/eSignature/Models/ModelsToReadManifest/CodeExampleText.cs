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

        [JsonProperty("CFREnabled")]
        public string CfrEnabled { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ResultsPageText")]
        public string ResultsPageText { get; set; }

        [JsonProperty("SkipForLanguages")]
        public string SkipForLanguages { get; set; }

        [JsonProperty("LinksToAPIMethod")]
        public List<LinkToApiMethods> LinksToApiMethod { get; } = new List<LinkToApiMethods>();

        [JsonProperty("CustomErrorTexts")]
        public List<CustomErrorTexts> CustomErrorTexts { get; } = new List<CustomErrorTexts>();

        [JsonProperty("RedirectsToOtherCodeExamples")]
        public List<RedirectsToOtherCodeExamples> RedirectsToOtherCodeExamples { get; } = new List<RedirectsToOtherCodeExamples>();

        [JsonProperty("Forms")]
        public List<Forms> Forms { get; } = new List<Forms>();

        [JsonProperty("AdditionalPage")]
        public List<AdditionalPage> AdditionalPages { get; } = new List<AdditionalPage>();
    }
}
