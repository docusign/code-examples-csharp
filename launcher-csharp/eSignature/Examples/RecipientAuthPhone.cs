// <copyright file="RecipientAuthPhone.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class RecipientAuthPhone
    {
        /// <summary>
        /// Creates an envelope and adds a recipient that is to be authenticated using either a phone call or an SMS (text) message.
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="countryAreaCode">Country code for the phone number used to verify the recipient/param>
        /// <param name="phoneNumber">Phone number used to verify the recipient</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public static string CreateEnvelopeWithRecipientUsingPhoneAuth(string signerEmail, string signerName, string accessToken, string basePath, string accountId, string countryAreaCode, string phoneNumber, string docPdf)
        {
            // Construct your API headers
            //ds-snippet-start:eSign20Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign20Step2

            //ds-snippet-start:eSign20Step3
            var accountsApi = new AccountsApi(docuSignClient);
            AccountIdentityVerificationResponse response = accountsApi.GetAccountIdentityVerification(accountId);
            var phoneAuthWorkflow = response.IdentityVerification.FirstOrDefault(x => x.DefaultName == "Phone Authentication");
            //ds-snippet-end:eSign20Step3

            if (phoneAuthWorkflow == null)
            {
                throw new ApiException(0, "IDENTITY_WORKFLOW_INVALID_ID");
            }

            string workflowId = phoneAuthWorkflow.WorkflowId;

            // Construct your envelope JSON body
            //ds-snippet-start:eSign20Step4
            EnvelopeDefinition env = new EnvelopeDefinition()
            {
                EnvelopeIdStamping = "true",
                EmailSubject = "Please Sign",
                EmailBlurb = "Sample text for email body",
                Status = "Sent",
            };

            byte[] buffer = System.IO.File.ReadAllBytes(docPdf);

            // Add a document
            Document doc1 = new Document()
            {
                DocumentId = "1",
                FileExtension = "pdf",
                Name = "Lorem",
                DocumentBase64 = Convert.ToBase64String(buffer),
            };

            // Create your signature tab
            env.Documents = new List<Document> { doc1 };
            SignHere signHere1 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorXOffset = "10",
                AnchorYOffset = "20",
            };

            // Tabs are set per recipient/signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 },
            };

            RecipientIdentityVerification workflow = new RecipientIdentityVerification()
            {
                WorkflowId = workflowId,
                InputOptions = new List<RecipientIdentityInputOption>
                {
                    new RecipientIdentityInputOption
                    {
                        Name = "phone_number_list",
                        ValueType = "PhoneNumberList",
                        PhoneNumberList = new List<RecipientIdentityPhoneNumber>
                        {
                            new RecipientIdentityPhoneNumber
                            {
                                Number = phoneNumber,
                                CountryCode = countryAreaCode,
                            },
                        },
                    },
                },
            };

            Signer signer1 = new Signer()
            {
                Name = signerName,
                Email = signerEmail,
                RoutingOrder = "1",
                Status = "Created",
                DeliveryMethod = "Email",
                RecipientId = "1", // represents your {RECIPIENT_ID},
                Tabs = signer1Tabs,
                IdentityVerification = workflow,
            };

            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer1 };
            env.Recipients = recipients;
            //ds-snippet-end:eSign20Step4

            // Call the eSignature REST API
            //ds-snippet-start:eSign20Step5
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            //ds-snippet-end:eSign20Step5

            return results.EnvelopeId;
        }
    }
}
