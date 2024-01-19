// <copyright file="SupportingTexts.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class SupportingTexts
    {
        [JsonProperty("HomePageText")]
        public string HomePageText { get; set; }

        [JsonProperty("ViewSourceFile")]
        public string ViewSourceFile { get; set; }

        [JsonProperty("APIMethodUsed")]
        public string ApiMethodUsed { get; set; }

        [JsonProperty("APIMethodUsedPlural")]
        public string ApiMethodUsedPlural { get; set; }

        [JsonProperty("SearchFailed")]
        public string SearchFailed { get; set; }

        [JsonProperty("SubmitButton")]
        public string SubmitButton { get; set; }

        [JsonProperty("ContinueButton")]
        public string ContinueButton { get; set; }

        [JsonProperty("HomeButton")]
        public string HomeButton { get; set; }

        [JsonProperty("LoginButton")]
        public string LoginButton { get; set; }

        [JsonProperty("LogoutButton")]
        public string LogoutButton { get; set; }

        [JsonProperty("ChangeAPITypeButton")]
        public string ChangeApiTypeButton { get; set; }

        [JsonProperty("WelcomeText")]
        public string WelcomeText { get; set; }

        [JsonProperty("CFRError")]
        public string CfrError { get; set; }

        [JsonProperty("LoginPage")]
        public LoginPage LoginPage { get; set; }

        [JsonProperty("SelectAPIPage")]
        public SelectApiPage SelectApiPage { get; set; }

        [JsonProperty("HelpingTexts")]
        public HelpingTexts HelpingTexts { get; set; }
    }
}