// <copyright file="CustomErrorTexts.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class CustomErrorTexts
    {
        [JsonProperty("ErrorMessageCheck")]
        public string ErrorMessageCheck { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
