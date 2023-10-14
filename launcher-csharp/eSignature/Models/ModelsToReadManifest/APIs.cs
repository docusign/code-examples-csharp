// <copyright file="APIs.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ApIs
    {
        [JsonProperty("Groups")]
        public List<ManifestGroup> Groups { get; } = new List<ManifestGroup>();

        [JsonProperty("Name")]
        public string Name { get; set; }
}
}
