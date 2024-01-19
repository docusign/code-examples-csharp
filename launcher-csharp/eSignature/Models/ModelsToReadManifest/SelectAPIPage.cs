// <copyright file="SelectAPIPage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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