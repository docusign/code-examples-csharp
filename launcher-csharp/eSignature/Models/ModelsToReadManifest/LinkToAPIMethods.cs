// <copyright file="LinkToAPIMethods.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class LinkToApiMethods
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("PathName")]
        public string PathName { get; set; }
    }
}
