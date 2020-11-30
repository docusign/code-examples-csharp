using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("Eg031")]
    public class Eg031BulkSendEnvelopesController : EgController
    {
        public Eg031BulkSendEnvelopesController(DSConfiguration config, IRequestItemsService requestItemsService) :
            base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg031";


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetProfile(RecipientModel signer1, RecipientModel carbonCopy1, RecipientModel signer2, RecipientModel carbonCopy2)
        {
            // Check the minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2. Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var bulkEnvelopesApi = new BulkEnvelopesApi(apiClient);

            // Construct request body
            var sendingList = this.MakeBulkSendList(signer1, carbonCopy1, signer2, carbonCopy2);

            try
            {
                // Step 3. Submit a bulk list
                var createBulkListResult = bulkEnvelopesApi.CreateBulkSendList(accountId, sendingList);

                
                // Step 4. Create an envelope
                var envelopeDefinition = new EnvelopeDefinition
                {
                    Documents = new List<Document>
                    {
                        new Document
                        {
                            DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.docDocx)),
                            Name = "Battle Plan",
                            FileExtension = "docx",
                            DocumentId = "1"
                        }
                    },
                    EnvelopeIdStamping = "true",
                    EmailSubject = "Please sign this document sent from the C# SDK",
                    Status = "created"
                };

                var envelopesApi = new EnvelopesApi(apiClient);
                var envelopeResults = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

                // Step 5. Attach your bulk list ID to the envelope
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

                // Step 6. Add placeholder recipients. 
                // These will be replaced by the details provided in the Bulk List uploaded during Step 3
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


                //Step 7. Initiate bulk send

                var bulkRequestResult = bulkEnvelopesApi.CreateBulkSendRequest(accountId, createBulkListResult.ListId,
                                                                               new BulkSendRequest
                                                                               {
                                                                                   EnvelopeOrTemplateId =
                                                                                       envelopeResults.EnvelopeId
                                                                               });

                //Wait until all requests sent. For 2000 recipients, it can take about 1h.
                System.Threading.Thread.Sleep(5000);


                Console.WriteLine("Bulk Batch ID: "+ bulkRequestResult.BatchId);

                // Step 8. Confirm successful batch send 
               var status = bulkEnvelopesApi.Get(accountId, bulkRequestResult.BatchId);

                ViewBag.h1 = "Bulk send envelope was successfully performed!";
                ViewBag.message = $@"Bulk request was sent to {status.Sent} user lists.";
            }
            catch (Exception ex)
            {
                ViewBag.h1 = "Bulk send envelope failed.";
                ViewBag.message = $@"Bulk request failed to send. Reason: {ex}.";
            }

            return View("example_done");
        }

        private BulkSendingList MakeBulkSendList(RecipientModel signer1, RecipientModel carbonCopy1, RecipientModel signer2, RecipientModel carbonCopy2)
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

                                Name = signer1.Name,
                                Email = signer1.Email,
                                RoleName    = "signer"
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy1.Name,
                                Email = carbonCopy1.Email,
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

                                Name = signer2.Name,
                                Email = signer2.Email,
                                RoleName    = "signer"
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy2.Name,
                                Email = carbonCopy2.Email,
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
