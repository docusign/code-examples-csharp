// <copyright file="CFRPart11EmbeddedSending.cs" company="DocuSign">
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

    public static class CfrPart11EmbeddedSending
    {
        private static readonly string ClientUserId = "12345";

        /// <summary>
        /// Checks if account is CFR Part 11 enabled
        /// </summary>
        /// <param name="accessToken">Access Token to make API calls</param>
        /// <param name="basePath">BasePath to make API calls</param>
        /// <param name="accountId">AccountId (GUID) for this account</param>
        /// <returns>True if CFR Part 11, false otherwise</returns>
        public static bool IsCfrPart11Account(string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Call the eSignature REST API
            AccountsApi accountsApi = new AccountsApi(docuSignClient);

            var accountSettingsInformation = accountsApi.ListSettings(accountId);

            return accountSettingsInformation.Require21CFRpt11Compliance == "true";
        }

        /// <summary>
        /// Creates an envelope and adds a recipient that is to be authenticated using either a phone call or an SMS (text) message.
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="countryAreaCode">Country code for the phone number used to verify the recipient</param>
        /// <param name="phoneNumber">Phone number used to verify the recipient</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="redirectUrl">Redirect URL</param>
        /// <returns> string URL for embedded signing</returns>
        public static string EmbeddedSigning(string signerEmail, string signerName, string accessToken, string basePath, string accountId, string countryAreaCode, string phoneNumber, string docPdf, string redirectUrl)
        {
            // Construct your API headers
            //ds-snippet-start:eSign41Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(docuSignClient);
            AccountIdentityVerificationResponse response = accountsApi.GetAccountIdentityVerification(accountId);
            var phoneAuthWorkflow = response.IdentityVerification.FirstOrDefault(x => x.DefaultName == "SMS for access & signatures");
            //ds-snippet-end:eSign41Step2

            if (phoneAuthWorkflow == null)
            {
                throw new ApiException(0, "IDENTITY_WORKFLOW_INVALID_ID");
            }

            string workflowId = phoneAuthWorkflow.WorkflowId;

            // Construct your envelope JSON body
            //ds-snippet-start:eSign41Step3
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
                AnchorXOffset = "20",
                AnchorYOffset = "-30",
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
                ClientUserId = ClientUserId,
            };

            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer1 };
            env.Recipients = recipients;
            //ds-snippet-end:eSign41Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign41Step4
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            //ds-snippet-end:eSign41Step4

            //ds-snippet-start:eSign41Step5
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, redirectUrl, ClientUserId);
            //ds-snippet-end:eSign41Step5

            // call the CreateRecipientView API
            //ds-snippet-start:eSign41Step6
            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, results.EnvelopeId, viewRequest);
            //ds-snippet-end:eSign41Step6

            return results1.Url;
        }

        //ds-snippet-start:eSign41Step5
        private static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {
            // Data for this method
            // signerEmail
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global
            RecipientViewRequest viewRequest = new RecipientViewRequest();

            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = returnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's
            // authentication, you can include authenticate steps from DocuSign.
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope.
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // Signing Ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app,
            // NOTE: The pings will only be sent if the pingUrl is an https address
            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600"; // seconds
                viewRequest.PingUrl = pingUrl; // optional setting
            }

            return viewRequest;
        }

        //ds-snippet-end:eSign41Step5
    }
}
