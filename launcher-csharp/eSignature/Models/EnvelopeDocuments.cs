// <copyright file="EnvelopeDocuments.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EnvelopeDocuments
    {
        public string EnvelopeId { get; set; }

        public List<EnvelopeDocItem> Documents { get; set; }
    }
}
