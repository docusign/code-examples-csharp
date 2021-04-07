using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
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
        /// <param name="signer2Name"> The signers recipient's name</param>
        /// <param name="signer2Email"> The signers recipient's email</param>
        /// <param name="carbonCopy2Name">The cc recipient's name</param>
        /// <param name="signer2Email"> The cc recipient's email</param>
        /// <param name="docDocx">The document</param>
        /// <returns>The status of sending</returns>
        public static BulkSendBatchStatus GetStatus(string signer1Name, string signer1Email, string carbonCopy1Name, string carbonCopy1Email, string signer2Name, string signer2Email, string carbonCopy2Name, string carbonCopy2Email, string accessToken, string basePath, string accountId, string docDocx, string envelopeIdStamping, string emailSubject)
        {
            // Step 1. Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var bulkEnvelopesApi = new BulkEnvelopesApi(apiClient);

            // Construct request body
            var sendingList = MakeBulkSendList(signer1Name,  signer1Email, carbonCopy1Name, carbonCopy1Email, signer2Name, signer2Email, carbonCopy2Name, carbonCopy2Email);


            // Step 2. Submit a bulk list
            var createBulkListResult = bulkEnvelopesApi.CreateBulkSendList(accountId, sendingList);


            // Step 3. Create an envelope
            var envelopeDefinition = new EnvelopeDefinition
            {
                Documents = new List<Document>
                    {
                        new Document
                        {
                            DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx)),
                            Name = "Battle Plan",
                            FileExtension = "docx",
                            DocumentId = "1"
                        }
                    },
                EnvelopeIdStamping = envelopeIdStamping,
                EmailSubject = emailSubject,
                Status = "created"
            };

            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            var envelopeResults = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

            // Step 4. Attach your bulk list ID to the envelope
            // Add an envelope custom field set to the value of your listId (EnvelopeCustomFields::create)
            // This Custom Field is used for tracking your Bulk Send via the Envelopes::Get method

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
                            Value = createBulkListResult.ListId //Adding the BULK_LIST_ID as an Envelope Custom Field
                        }
                    }
            };
            envelopesApi.CreateCustomFields(accountId, envelopeResults.EnvelopeId, fields);

            // Step 5. Add placeholder recipients. 
            // These will be replaced by the details provided in the Bulk List uploaded during Step 2
            // Note: The name / email format used is:
            // Name: Multi Bulk Recipients::{rolename}
            // Email: MultiBulkRecipients-{rolename}@docusign.com

            var recipients = new Recipients
            {
                Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Name = "Multi Bulk Recipient::signer",
                            Email = "multiBulkRecipients-signer@docusign.com",
                            RoleName = "signer",
                            RoutingOrder = "1",
                            Status = "sent",
                            DeliveryMethod = "Email",
                            RecipientId = "1",
                            RecipientType = "signer"
                        },
                        new Signer
                        {
                            Name = "Multi Bulk Recipient::cc",
                            Email = "multiBulkRecipients-cc@docusign.com",
                            RoleName = "cc",
                            RoutingOrder = "1",
                            Status = "sent",
                            DeliveryMethod = "Email",
                            RecipientId = "2",
                            RecipientType = "cc"
                        }
                    }
            };
            envelopesApi.CreateRecipient(accountId, envelopeResults.EnvelopeId, recipients);

            // Step 6. Initiate bulk send
            var bulkRequestResult = bulkEnvelopesApi.CreateBulkSendRequest(accountId, createBulkListResult.ListId, new BulkSendRequest { EnvelopeOrTemplateId = envelopeResults.EnvelopeId });
            // TODO: instead of waiting 5 seconds, consider using the Asynchrnous method
            System.Threading.Thread.Sleep(5000);

            // Step 7. Confirm successful batch send 
            return bulkEnvelopesApi.GetBulkSendBatchStatus(accountId, bulkRequestResult.BatchId);
        }

        private static BulkSendingList MakeBulkSendList(string signer1Name, string signer1Email, string carbonCopy1Name, string carbonCopy1Email, string signer2Name, string signer2Email, string carbonCopy2Name, string carbonCopy2Email)
        {
            return new BulkSendingList
            {
                BulkCopies = new List<BulkSendingCopy> {
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {

                                Name = signer1Name,
                                Email = signer1Email,
                                RoleName    = "signer"
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy1Name,
                                Email = carbonCopy1Email,
                                RoleName    = "cc"
                            }
                        }
                    },
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {

                                Name = signer2Name,
                                Email = signer2Email,
                                RoleName    = "signer"
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy2Name,
                                Email = carbonCopy2Email,
                                RoleName    = "cc"
                            }
                        }
                    }
                },
                Name = "sample.csv"
            };
        }
    }
}
