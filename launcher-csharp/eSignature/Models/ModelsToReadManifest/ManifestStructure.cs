﻿// <copyright file="ManifestStructure.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ManifestStructure
    {
        [JsonProperty("SupportingTexts")]
        public SupportingTexts SupportingTexts { get; set; }

        [JsonProperty("APIs")]
        public List<ApIs> ApIs { get; } = new List<ApIs>();
    }
}
