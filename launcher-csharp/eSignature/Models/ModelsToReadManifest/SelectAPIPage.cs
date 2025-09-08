// <copyright file="SelectAPIPage.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class SelectApiPage
    {
        [JsonProperty("SelectAPIcHeader")]
        public string SelectApiHeader { get; set; }

        [JsonProperty("SelectAPIButton")]
        public string SelectApiButton { get; set; }
    }
}