// <copyright file="HelpingTexts.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class HelpingTexts
    {
        [JsonProperty("EmailWontBeShared")]
        public string EmailWontBeShared { get; set; }

        [JsonProperty("PhoneNumberWontBeShared")]
        public string PhoneNumberWontBeShared { get; set; }

        [JsonProperty("CCEmailShouldDifferFromSigner")]
        public string CCEmailShouldDifferFromSigner { get; set; }

        [JsonProperty("AccessCodeText")]
        public string AccessCodeText { get; set; }

        [JsonProperty("CountryCodeText")]
        public string CountryCodeText { get; set; }

        [JsonProperty("ChooseDateInTheFuture")]
        public string ChooseDateInTheFuture { get; set; }

        [JsonProperty("PhoneNumberWillBeNotified")]
        public string PhoneNumberWillBeNotified { get; set; }
    }
}