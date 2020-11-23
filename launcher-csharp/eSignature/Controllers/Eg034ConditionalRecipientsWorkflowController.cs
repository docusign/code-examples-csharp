using System;
using System.Collections.Generic;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Route("Eg034")]
    public class Eg034ConditionalRecipientsWorkflowController : EgController
    {
        public Eg034ConditionalRecipientsWorkflowController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg034";

        [HttpPost]
        public IActionResult Create(RecipientModel recipient1, RecipientModel conditionalRecipient1, RecipientModel conditionalRecipient2)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);

            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            string basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken;
            string accountId = RequestItemsService.Session.AccountId;

            // Step 2. Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);

            // Step 3. Construct request body
            var envelope = CreateEnvelope(recipient1, conditionalRecipient1, conditionalRecipient2);
            EnvelopeSummary results;

            try
            {
                // Step 4. Call the eSignature API
                results = envelopesApi.CreateEnvelope(accountId, envelope);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;

                if (apiException.Message.Contains("WORKFLOW_UPDATE_RECIPIENTROUTING_NOT_ALLOWED"))
                {
                    ViewBag.errorMessage = "Update to the workflow with recipient routing is not allowed for your account!";
                    ViewBag.errorInformation = "Please contact with our <a href='https://developers.docusign.com/support/' target='_blank'>support team</a> to resolve this issue.";
                }
                else
                {
                    ViewBag.errorMessage = apiException.Message;
                }

                return View("Error");
            }

            // Process results
            ViewBag.h1 = "The envelope was created successfully!";
            ViewBag.message = "Results from the Envelopes::create method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }

        private EnvelopeDefinition CreateEnvelope(RecipientModel recipient1, RecipientModel conditionalRecipient1, RecipientModel conditionalRecipient2)
        {
            var document = new Document()
            {
                DocumentBase64 = "VGhhbmtzIGZvciByZXZpZXdpbmcgdGhpcyEKCldlJ2xsIG1vdmUgZm9yd2FyZCBhcyBzb29uIGFzIHdlIGhlYXIgYmFjay4=",
                DocumentId = "1",
                FileExtension = "txt",
                Name = "Welcome"
            };

            var conditionalRecipientRule = new ConditionalRecipientRule()
            {
                RecipientId = "2",
                Order = "2",
                RecipientGroup = new RecipientGroup()
                {
                    GroupName = "Approver",
                    GroupMessage = "Members of this group approve a workflow",
                    Recipients = new List<RecipientOption>()
                    {
                        new RecipientOption(conditionalRecipient1.Email, conditionalRecipient1.Name, "signer2a", "Signer when not checked"),
                        new RecipientOption(conditionalRecipient2.Email, conditionalRecipient2.Name, "signer2b", "Signer when not checked")
                    }
                },
                Conditions = new List<ConditionalRecipientRuleCondition>
                {
                    new ConditionalRecipientRuleCondition
                    {
                        RecipientLabel = "signer2a",
                        Order = "1",
                        Filters = new List<ConditionalRecipientRuleFilter>()
                        {
                            new ConditionalRecipientRuleFilter
                            {
                                Scope = "tabs",
                                RecipientId = "1",
                                TabId = "ApprovalTab",
                                Operator = "equals",
                                Value = "false",
                                TabLabel = "ApproveWhenChecked"
                            }
                        }
                    },
                    new ConditionalRecipientRuleCondition
                    {
                        RecipientLabel = "signer2b",
                        Order = "2",
                        Filters = new List<ConditionalRecipientRuleFilter>()
                        {
                            new ConditionalRecipientRuleFilter
                            {
                                Scope = "tabs",
                                RecipientId = "1",
                                TabId = "ApprovalTab",
                                Operator = "equals",
                                Value = "true",
                                TabLabel = "ApproveWhenChecked"
                            }
                        }
                    }
                }
            };

            var workflowStep = new WorkflowStep()
            {
                Action = "pause_before",
                TriggerOnItem = "routing_order",
                ItemId = "2",
                Status = "pending",
                RecipientRouting = new RecipientRouting()
                {
                    Rules = new RecipientRules()
                    {
                        ConditionalRecipients = new List<ConditionalRecipientRule>
                        {
                            conditionalRecipientRule
                        }
                    }
                }
            };

            var signer1 = new Signer()
            {
                Email = recipient1.Email,
                Name = recipient1.Name,
                RecipientId = "1",
                RoutingOrder = "1",
                RoleName = "Purchaser",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>()
                    {
                        new SignHere()
                        {
                            Name = "SignHere",
                            DocumentId = "1",
                            PageNumber = "1",
                            TabLabel = "PurchaserSignature",
                            XPosition = "200",
                            YPosition = "200"
                        }
                    },
                    CheckboxTabs = new List<Checkbox>()
                    {
                        new Checkbox()
                        {
                            Name = "ClickToApprove",
                            Selected = "false",
                            DocumentId = "1",
                            PageNumber = "1",
                            TabLabel = "ApproveWhenChecked",
                            XPosition = "50",
                            YPosition = "50"
                        }
                    }
                }
            };

            var signer2 = new Signer()
            {
                Email = "placeholder@example.com",
                Name = "Approver",
                RecipientId = "2",
                RoutingOrder = "2",
                RoleName = "Approver",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere()
                        {
                            Name = "SignHere",
                            DocumentId = "1",
                            PageNumber = "1",
                            RecipientId = "2",
                            TabLabel = "ApproverSignature",
                            XPosition = "300",
                            YPosition = "200"
                        }
                    }
                }
            };

            var envelopeDefinition = new EnvelopeDefinition()
            {
                Documents = new List<Document> { document },
                EmailSubject = "ApproveIfChecked",
                Workflow = new Workflow { WorkflowSteps = new List<WorkflowStep> { workflowStep } },
                Recipients = new Recipients { Signers = new List<Signer> { signer1, signer2 } },
                Status = "Sent"
            };

            return envelopeDefinition;
        }
    }
}