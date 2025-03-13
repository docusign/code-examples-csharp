// <copyright file="ConnectionInstances.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ConnectedFields.Models
{
    using Newtonsoft.Json;

    public class ConnectionInstances
    {
        [JsonProperty("connectionKey")]
        public string ConnectionKey { get; set; }

        [JsonProperty("connectionValue")]
        public string ConnectionValue { get; set; }
    }
}
