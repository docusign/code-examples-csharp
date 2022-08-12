// <copyright file="ListEnvelopeDocuments.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class ListEnvelopeDocuments
    {
        public class EnvelopeDocItem
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string DocumentId { get; set; }
        }

        public class EnvelopeDocuments
        {
            public string EnvelopeId { get; set; }

            public List<EnvelopeDocItem> Documents { get; set; }
        }
    }
}
