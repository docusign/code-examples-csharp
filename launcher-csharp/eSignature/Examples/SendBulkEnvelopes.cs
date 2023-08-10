// <copyright file="SendBulkEnvelopes.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class SendBulkEnvelopes
    {
        /// <summary>
        /// Sends the bulk envelopes
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="signer1Name"> The signer recipient's name</param>
        /// <param name="signer1Email"> The signer recipient's email</param>
        /// <param name="carbonCopy1Name">The cc recipient's name</param>
        /// <param name="signer1Email"> The cc recipient's email</param>
        /// <param name="signer2Name"> The signers recipient's name.</param>
        /// <param name="signer2Email"> The signers recipient's email</param>
        /// <param name="carbonCopy2Name">The cc recipient's name</param>
        /// <param name="carbonCopy2Email"> The cc recipient's email</param>
        /// <param name="docDocx">The document</param>
        /// <returns>The status of sending</returns>
        public static BulkSendBatchStatus GetStatus(string signer1Name, string signer1Email, string carbonCopy1Name, string carbonCopy1Email, string signer2Name, string signer2Email, string carbonCopy2Name, string carbonCopy2Email, string accessToken, string basePath, string accountId, string docDocx, string envelopeIdStamping, string emailSubject)
        {
            //ds-snippet-start:eSign31Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var bulkEnvelopesApi = new BulkEnvelopesApi(docuSignClient);
            //ds-snippet-end:eSign31Step2

            //ds-snippet-start:eSign31Step3
            var sendingList = MakeBulkSendList(signer1Name, signer1Email, carbonCopy1Name, carbonCopy1Email, signer2Name, signer2Email, carbonCopy2Name, carbonCopy2Email);

            var createBulkListResult = bulkEnvelopesApi.CreateBulkSendList(accountId, sendingList);
            //ds-snippet-end:eSign31Step3

            //ds-snippet-start:eSign31Step4
            var envelopeDefinition = new EnvelopeDefinition
            {
                Documents = new List<Document>
                    {
                        new Document
                        {
                            DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx)),
                            Name = "Lorem Ipsum",
                            FileExtension = "pdf",
                            DocumentId = "1",
                        },
                    },
                EnvelopeIdStamping = envelopeIdStamping,
                EmailSubject = emailSubject,
                Status = "created",
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Name = "Multi Bulk Recipient::signer",
                            Email = "multiBulkRecipients-signer@docusign.com",
                            RoleName = "signer",
                            RoutingOrder = "1",
                            Status = "created",
                            DeliveryMethod = "Email",
                            RecipientId = "1",
                            RecipientType = "signer",
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        AnchorString = "/sn1/",
                                        AnchorUnits = "pixels",
                                        AnchorYOffset = "10",
                                        AnchorXOffset = "20",
                                    },
                                },
                            },
                        },
                    },
                    CarbonCopies = new List<CarbonCopy>
                    {
                        new CarbonCopy
                        {
                            Name = "Multi Bulk Recipient::cc",
                            Email = "multiBulkRecipients-cc@docusign.com",
                            RoleName = "cc",
                            RoutingOrder = "2",
                            Status = "created",
                            DeliveryMethod = "Email",
                            RecipientId = "2",
                            RecipientType = "cc",
                        },
                    },
                },
            };

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            var envelopeResults = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
            //ds-snippet-end:eSign31Step4

            // Attach your bulk list ID to the envelope
            // Add an envelope custom field set to the value of your listId (EnvelopeCustomFields::create)
            // This Custom Field is used for tracking your Bulk Send via the Envelopes::Get method
            //ds-snippet-start:eSign31Step5
            var fields = new CustomFields
            {
                ListCustomFields = new List<ListCustomField> { },

                TextCustomFields = new List<TextCustomField>
                    {
                        new TextCustomField
                        {
                            Name = "mailingListId",
                            Required = "false",
                            Show = "false",
                            Value = createBulkListResult.ListId, // Adding the BULK_LIST_ID as an Envelope Custom Field
                        },
                    },
            };
            envelopesApi.CreateCustomFields(accountId, envelopeResults.EnvelopeId, fields);
            //ds-snippet-end:eSign31Step5

            //ds-snippet-start:eSign31Step6
            var bulkRequestResult = bulkEnvelopesApi.CreateBulkSendRequest(accountId, createBulkListResult.ListId, new BulkSendRequest { EnvelopeOrTemplateId = envelopeResults.EnvelopeId });
            //ds-snippet-end:eSign31Step6

            // TODO: instead of waiting 10 seconds, consider using the Asynchrnous method
            System.Threading.Thread.Sleep(10000);

            //ds-snippet-start:eSign31Step7
            return bulkEnvelopesApi.GetBulkSendBatchStatus(accountId, bulkRequestResult.BatchId);
            //ds-snippet-end:eSign31Step7
        }

        // step 3-2 start
        public static BulkSendingList MakeBulkSendList(string signer1Name, string signer1Email, string carbonCopy1Name, string carbonCopy1Email, string signer2Name, string signer2Email, string carbonCopy2Name, string carbonCopy2Email)
        {
            return new BulkSendingList
            {
                BulkCopies = new List<BulkSendingCopy>
                {
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {
                                Name = signer1Name,
                                Email = signer1Email,
                                RoleName = "signer",
                            },
                            new BulkSendingCopyRecipient
                            {
                                Name = carbonCopy1Name,
                                Email = carbonCopy1Email,
                                RoleName = "cc",
                            },
                        },
                    },
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {
                                Name = signer2Name,
                                Email = signer2Email,
                                RoleName = "signer",
                            },
                            new BulkSendingCopyRecipient
                            {
                                Name = carbonCopy2Name,
                                Email = carbonCopy2Email,
                                RoleName = "cc",
                            },
                        },
                    },
                },
                Name = "sample.csv",
            };
        }

        // step 3-2 end
    }
}
