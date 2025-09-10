// <copyright file="Inputs.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class Inputs
    {
        [JsonProperty("InputName")]
        public string InputName { get; set; }

        [JsonProperty("InputPlaceholder")]
        public string InputPlaceholder { get; set; }
    }
}
