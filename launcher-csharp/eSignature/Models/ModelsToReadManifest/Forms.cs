﻿// <copyright file="Forms.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Forms
    {
        [JsonProperty("FormName")]
        public string FormName { get; set; }

        [JsonProperty("Inputs")]
        public List<Inputs> Inputs { get; } = new List<Inputs>();
    }
}
