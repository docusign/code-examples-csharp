// <copyright file="Tabs.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ConnectedFields.Models
{
    using Newtonsoft.Json;

    public class Tabs
    {
        [JsonProperty("extensionData")]
        public ExtensionData ExtensionData { get; set; }

        [JsonProperty("tabType")]
        public string TabType { get; set; }

        [JsonProperty("tabLabel")]
        public string TabLabel { get; set; }
    }
}
