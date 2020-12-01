using System;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static DocuSign.eSign.Api.EnvelopesApi;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Route("Eg033")]
    public class Eg033UnpauseSignatureWorkflowController : EgController
    {
        public Eg033UnpauseSignatureWorkflowController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Unpause a signature workflow";
        }

        public override string EgName => "Eg033";

        [HttpPost]
        public IActionResult Update()
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
            var envelope = CreateEnvelope();

            // Step 4. Call the eSignature API
            var updateOptions = new UpdateOptions() { resendEnvelope = "true" };
            var results = envelopesApi.Update(accountId, RequestItemsService.PausedEnvelopeId, envelope, updateOptions);

            // Process results
            ViewBag.h1 = "The envelope was updated successfully!";
            ViewBag.message = "Results from the Envelopes::update method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }

        private Envelope CreateEnvelope()
        {
            return new Envelope
            {
                Workflow = new Workflow()
                {
                    WorkflowStatus = "in_progress"
                }
            };
        }
    }
}
