// <copyright file="AdditionalPage.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
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
