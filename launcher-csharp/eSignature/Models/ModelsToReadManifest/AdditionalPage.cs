// <copyright file="AdditionalPage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class AdditionalPage
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ResultsPageText")]
        public string ResultsPageText { get; set; }
    }
}
