// <copyright file="ScheduledSending.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class ScheduledSending
    {
        /// <summary>
        /// Creates an envelope that would include two documents and add a signer and cc recipients to be notified via email
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public static string ScheduleEnvelope(string signerEmail, string signerName, string accessToken, string basePath, string accountId, string docPdf, DateTime resumeDate)
        {
            EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, docPdf, resumeDate);

            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-start:eSign35Step3
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            //ds-snippet-end:eSign35Step3
            return results.EnvelopeId;
        }

        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string docPdf, DateTime resumeDate)
        {
            // Data for this method
            // signerEmail
            // signerName
            // docPdf
            // resumeDate

            // document 1 (pdf) has tag /sn1/
            //
            //ds-snippet-start:eSign35Step2
            // The envelope has a single recipient.
            // recipient 1 - signer
            // read file from a local directory
            // The reads could raise an exception if the file is not available!
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

            // Add the workflow rule that sets the schedule for the envelope to be sent
            Workflow workflow = new Workflow { ScheduledSending = new DocuSign.eSign.Model.ScheduledSending() };
            var rule = new EnvelopeDelayRule();
            rule.ResumeDate = resumeDate.ToString("MM/dd/yyyy");
            workflow.ScheduledSending.Rules = new List<EnvelopeDelayRule> { rule };
            env.Workflow = workflow;

            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // Create signHere fields (also known as tabs) on the document,
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
                SignHereTabs = new List<SignHere> { signHere },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
            };
            env.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = "sent";
            //ds-snippet-end:eSign35Step2
            return env;
        }
    }
}
