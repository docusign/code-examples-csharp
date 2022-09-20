// <copyright file="AdditionalPage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
