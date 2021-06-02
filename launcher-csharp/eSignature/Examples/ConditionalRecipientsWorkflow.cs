using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System.Collections.Generic;

namespace eSignature.Examples
{
    public class ConditionalRecipientsWorkflow
    {
        /// <summary>
        /// Unpauses signature workflow
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="recipient1Email">The signer email</param>
        /// <param name="recipient1Name">The signer name</param>
        /// <param name="conditionalRecipient1Email">The first conditional signer email</param>
        /// <param name="conditionalRecipient1Name">The first conditional signer name</param>
        /// <param name="conditionalRecipient2Email">The second conditional signer email</param>
        /// <param name="conditionalRecipient2Name">The second conditional signer name</param>
        /// <returns>The update summary of the envelopes</returns>
        public static EnvelopeSummary SendEnvelope(string accessToken, string basePath, string accountId, string recipient1Email, string recipient1Name, string conditionalRecipient1Email, string conditionalRecipient1Name, string conditionalRecipient2Email, string conditionalRecipient2Name)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Construct request body
            var envelope = MakeEnvelope(recipient1Email, recipient1Name, conditionalRecipient1Email,
                conditionalRecipient1Name, conditionalRecipient2Email, conditionalRecipient2Name);

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            return envelopesApi.CreateEnvelope(accountId, envelope);
        }

        private static EnvelopeDefinition MakeEnvelope(string recipient1Email, string recipient1Name, string conditionalRecipient1Email, string conditionalRecipient1Name, string conditionalRecipient2Email, string conditionalRecipient2Name)
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
                        new RecipientOption(conditionalRecipient1Email, conditionalRecipient1Name, "signer2a", "Signer when not checked"),
                        new RecipientOption(conditionalRecipient2Email, conditionalRecipient2Name, "signer2b", "Signer when not checked")
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
                                TabLabel = "ApproveWhenChecked",
                                TabType = "checkbox"
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
                                TabLabel = "ApproveWhenChecked",
                                TabType = "checkbox"
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
                Email = recipient1Email,
                Name = recipient1Name,
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
