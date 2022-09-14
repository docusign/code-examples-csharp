// <copyright file="LoginPage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class SelectAPIPage
    {
        [JsonProperty("SelectAPIcHeader")]
        public string SelectAPIHeader { get; set; }

        [JsonProperty("SelectAPIButton")]
        public string SelectAPIButton { get; set; }
    }
}