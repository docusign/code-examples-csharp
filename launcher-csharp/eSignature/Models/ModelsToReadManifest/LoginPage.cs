﻿// <copyright file="LoginPage.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using Newtonsoft.Json;

    public class LoginPage
    {
        [JsonProperty("LoginHeader")]
        public string LoginHeader { get; set; }

        [JsonProperty("LoginButton")]
        public string LoginButton { get; set; }

        [JsonProperty("LoginHelperText")]
        public string LoginHelperText { get; set; }
    }
}