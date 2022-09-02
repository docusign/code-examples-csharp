// <copyright file="ManifestStructure.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ManifestStructure
    {
        [JsonProperty("SupportingTexts")]
        public SupportingTexts SupportingTexts { get; }

        [JsonProperty("Groups")]
        public List<ManifestGroup> Groups { get; } = new List<ManifestGroup>();
    }
}
