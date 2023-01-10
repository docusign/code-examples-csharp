// <copyright file="Inputs.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
