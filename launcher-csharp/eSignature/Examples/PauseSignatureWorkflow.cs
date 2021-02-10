using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System.Collections.Generic;

namespace eSignature.Examples
{
    public class PauseSignatureWorkflow
    {
        /// <summary>
        /// Pauses workflow of signature
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="recipient1Email">The first recipient's email</param>
        /// <param name="recipient1Name">The first recipient's name</param>
        /// <param name="recipient2Email">The second recipient's email</param>
        /// <param name="recipient2Name">The second recipient's name</param>
        /// <returns>The summary of the envelopes</returns>
        public static EnvelopeSummary PauseWorkflow(string recipient1Email, string recipient1Name, string recipient2Email, string recipient2Name, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Construct request body
            var envelope = MakeEnvelope(recipient1Email, recipient1Name, recipient2Email, recipient2Name);

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            return envelopesApi.CreateEnvelope(accountId, envelope);
        }

        private static EnvelopeDefinition MakeEnvelope(string recipient1Email, string recipient1Name, string recipient2Email, string recipient2Name)
        {
            var document = new Document()
            {
                DocumentBase64 = "DQoNCg0KDQoJCVdlbGNvbWUgdG8gdGhlIERvY3VTaWduIFJlY3J1aXRpbmcgRXZlbnQNCgkJDQoJCQ0KCQlQbGVhc2UgU2lnbiBpbiENCgkJDQoJCQ0KCQk=",
                DocumentId = "1",
                FileExtension = "txt",
                Name = "Welcome"
            };

            var workflowStep = new WorkflowStep()
            {
                Action = "pause_before",
                TriggerOnItem = "routing_order",
                ItemId = "2"
            };

            var signer1 = new Signer()
            {
                Email = recipient1Email,
                Name = recipient1Name,
                RecipientId = "1",
                RoutingOrder = "1",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere()
                        {
                            DocumentId = "1",
                            PageNumber = "1",
                            TabLabel = "Sign Here",
                            XPosition = "200",
                            YPosition = "200"
                        }
                    }
                }
            };

            var signer2 = new Signer()
            {
                Email = recipient2Email,
                Name = recipient2Name,
                RecipientId = "2",
                RoutingOrder = "2",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere()
                        {
                            DocumentId = "1",
                            PageNumber = "1",
                            TabLabel = "Sign Here",
                            XPosition = "300",
                            YPosition = "200"
                        }
                    }
                }
            };

            var envelopeDefinition = new EnvelopeDefinition()
            {
                Documents = new List<Document> { document },
                EmailSubject = "EnvelopeWorkflowTest",
                Workflow = new Workflow { WorkflowSteps = new List<WorkflowStep> { workflowStep } },
                Recipients = new Recipients { Signers = new List<Signer> { signer1, signer2 } },
                Status = "Sent"
            };

            return envelopeDefinition;
        }
    }
}
