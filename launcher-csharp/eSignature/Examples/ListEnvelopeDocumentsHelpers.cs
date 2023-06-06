// <copyright file="ListEnvelopeDocumentsHelpers.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System.Collections.Generic;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    internal static class ListEnvelopeDocumentsHelpers
    {
        /// <summary>
        /// Gets a list of all the documents for a specific envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>An object containing information about all the documents in the envelopes</returns>
        public static EnvelopeDocuments GetDocuments(string accessToken, string basePath, string accountId, string envelopeId)
        {
            //ds-snippet-start:eSign6Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign6Step2

            //ds-snippet-start:eSign6Step3
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeDocumentsResult results = envelopesApi.ListDocuments(accountId, envelopeId);
            //ds-snippet-end:eSign6Step3

            List<EnvelopeDocItem> envelopeDocItems = new List<EnvelopeDocItem>
            {
                new EnvelopeDocItem { Name = "Combined", Type = "content", DocumentId = "combined" },
                new EnvelopeDocItem { Name = "Zip archive", Type = "zip", DocumentId = "archive" },
            };

            foreach (EnvelopeDocument doc in results.EnvelopeDocuments)
            {
                envelopeDocItems.Add(new EnvelopeDocItem
                {
                    DocumentId = doc.DocumentId,
                    Name = doc.DocumentId == "certificate" ? "Certificate of completion" : doc.Name,
                    Type = doc.Type,
                });
            }

            EnvelopeDocuments envelopeDocuments = new EnvelopeDocuments
            {
                EnvelopeId = envelopeId,
                Documents = envelopeDocItems,
            };

            return envelopeDocuments;
        }
    }
}