// <copyright file="InPersonSigning.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class InPersonSigning
    {
        /// <summary>
        /// Creates a new envelope, adds a single document and an in-person signer and generates a url that is used for embedded signing.
        /// </summary>
        /// <param name="hostEmail">Email address for the host</param>
        /// <param name="hostName">Full name of the host</param>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="returnUrl">URL user will be redirected to after they sign</param>
        /// <param name="pingUrl">URL that DocuSign will be able to ping to incdicate signing session is active</param>
        /// <returns>The URL for the embedded signing</returns>
        public static string SendEnvelopeForInPersonSigning(
            string hostEmail,
            string hostName,
            string signerEmail,
            string accessToken,
            string basePath,
            string accountId,
            string docPdf,
            string returnUrl,
            string pingUrl = null)
        {
            EnvelopeDefinition envelopeDefinition = PrepareEnvelope(hostEmail, hostName, signerEmail, docPdf);

            //ds-snippet-start:eSign39Step3
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);

            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
            //ds-snippet-end:eSign39Step3
            //ds-snippet-start:eSign39Step5
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(hostEmail, hostName, returnUrl, pingUrl);
            ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, viewRequest);
            //ds-snippet-end:eSign39Step5
            return viewUrl.Url;
        }

        //ds-snippet-start:eSign39Step4
        private static RecipientViewRequest MakeRecipientViewRequest(
            string hostEmail,
            string hostName,
            string returnUrl,
            string pingUrl = null)
        {
            var recipientViewRequest = new RecipientViewRequest
            {
                ReturnUrl = returnUrl + "?state=123",
                AuthenticationMethod = "none",
                Email = hostEmail,
                UserName = hostName,
            };

            if (pingUrl != null)
            {
                recipientViewRequest.PingFrequency = "600";
                recipientViewRequest.PingUrl = pingUrl;
            }

            return recipientViewRequest;
        }

        //ds-snippet-end:eSign39Step4

        //ds-snippet-start:eSign39Step2
        private static EnvelopeDefinition PrepareEnvelope(string hostEmail, string hostName, string signerName, string docPdf)
        {
            byte[] fileContentInBytes = System.IO.File.ReadAllBytes(docPdf);

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please host this in-person signing session",
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(fileContentInBytes),
                        Name = "Lorem Ipsum",
                        FileExtension = "pdf",
                        DocumentId = "1",
                    },
                },
            };

            InPersonSigner inPersonSigner = new InPersonSigner
            {
                HostEmail = hostEmail,
                HostName = hostName,
                SignerName = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere
                        {
                            AnchorString = "/sn1/",
                            AnchorUnits = "pixels",
                            AnchorXOffset = "20",
                            AnchorYOffset = "10",
                        },
                    },
                },
            };

            envelopeDefinition.Recipients = new Recipients
            {
                InPersonSigners = new List<InPersonSigner>
                {
                    inPersonSigner,
                },
            };

            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }

        //ds-snippet-end:eSign39Step2
    }
}
