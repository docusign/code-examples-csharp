// <copyright file="AdditionalPage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Forms
    {
        [JsonProperty("FormName")]
        public string FormName { get; set; }

        [JsonProperty("Inputs")]
        public List<Inputs> Inputs { get; } = new List<Inputs>();
    }
}
