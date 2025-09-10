﻿// <copyright file="ApplyBrandToEnvelope.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public class ApplyBrandToEnvelope
    {
        /// <summary>
        /// Applies a brand to the envelope
        /// </summary>
        /// <param name="brandId">The brand ID</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <returns>The summary of the envelopes</returns>
        public static EnvelopeSummary CreateEnvelopeWithBranding(string signerEmail, string signerName, string brandId, string accessToken, string basePath, string accountId, string status, string docPdf)
        {
            // Construct your API headers
            //ds-snippet-start:eSign29Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign29Step2

            // Construct your request body
            //ds-snippet-start:eSign29Step3
            EnvelopeDefinition env = CreateEnvelope(signerEmail, signerName, brandId, status, docPdf);
            //ds-snippet-end:eSign29Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign29Step4
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);

            return envelopesApi.CreateEnvelope(accountId, env);
            //ds-snippet-end:eSign29Step4
        }

        //ds-snippet-start:eSign29Step3
        public static EnvelopeDefinition CreateEnvelope(string signerEmail, string signerName, string brandId, string status, string docPdf)
        {
            string docPdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));

            // create the envelope definition
            EnvelopeDefinition env = new EnvelopeDefinition();
            env.EmailSubject = "Please sign this document set";

            // Create document objects, one per document
            Document doc = new Document
            {
                DocumentBase64 = docPdfBytes,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "pdf",
                DocumentId = "1",
            };

            // The order in the docs array determines the order in the envelope
            env.Documents = new List<Document> { doc };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings.
            SignHere signHere = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20",
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere, },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
            };
            env.Recipients = recipients;
            env.Status = status;

            // Set the brand id.
            env.BrandId = brandId;

            return env;
        }

        //ds-snippet-end:eSign29Step3
    }
}
