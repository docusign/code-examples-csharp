// <copyright file="ExtensionApp.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ConnectedFields.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ExtensionApp
    {
        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("tabs")]
        public List<Tabs> Tabs { get; set; }
    }
}
