// <copyright file="RedirectsToOtherCodeExamples.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
