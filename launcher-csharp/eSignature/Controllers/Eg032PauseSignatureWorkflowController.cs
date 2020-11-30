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
    [Route("Eg032")]
    public class Eg032PauseSignatureWorkflowController : EgController
    {
        public Eg032PauseSignatureWorkflowController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Pause a signature workflow";
        }

        public override string EgName => "Eg032";

        [HttpPost]
        public IActionResult Create(RecipientModel recipient1, RecipientModel recipient2)
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
            var envelope = CreateEnvelope(recipient1, recipient2);

            // Step 4. Call the eSignature API
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelope);

            // Process results
            RequestItemsService.PausedEnvelopeId = results.EnvelopeId;
            ViewBag.h1 = "The envelope was created successfully!";
            ViewBag.message = "Results from the Envelopes::create method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }

        private EnvelopeDefinition CreateEnvelope(RecipientModel recipient1, RecipientModel recipient2)
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
                Email = recipient1.Email,
                Name = recipient1.Name,
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
                Email = recipient2.Email,
                Name = recipient2.Name,
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