// <copyright file="RedirectsToOtherCodeExamples.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class RedirectsToOtherCodeExamples
    {
        [JsonProperty("CodeExampleToRedirectTo")]
        public int CodeExampleToRedirectTo { get; set; }

        [JsonProperty("RedirectText")]
        public string RedirectText { get; set; }
    }
}
