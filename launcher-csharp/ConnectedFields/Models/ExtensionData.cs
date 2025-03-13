// <copyright file="ExtensionData.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ConnectedFields.Models
{
    using Newtonsoft.Json;

    public class ExtensionData
    {
        [JsonProperty("extensionGroupId")]
        public ExtensionData ExtensionGroupId { get; set; }

        [JsonProperty("actionInputKey")]
        public string ActionInputKey { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("actionContract")]
        public string ActionContract { get; set; }

        [JsonProperty("extensionName")]
        public string ExtensionName { get; set; }

        [JsonProperty("actionName")]
        public string ActionName { get; set; }

        [JsonProperty("extensionContract")]
        public string ExtensionContract { get; set; }

        [JsonProperty("extensionPolicy")]
        public string ExtensionPolicy { get; set; }

        [JsonProperty("requiredForExtension")]
        public bool RequiredForExtension { get; set; }

        [JsonProperty("connectionInstances")]
        public ConnectionInstances ConnectionInstances { get; set; }
    }
}
