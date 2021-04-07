using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using eSignature.Examples;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Area("eSignature")]
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

            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken;
            string accountId = RequestItemsService.Session.AccountId;

            // Call the eSignature API
            var results = UnpauseSignatureWorkflow.UnpauseWorkflow(accessToken, basePath, accountId,
                RequestItemsService.PausedEnvelopeId);

            // Process results
            ViewBag.h1 = "The envelope was updated successfully!";
            ViewBag.message = "Results from the Envelopes::update method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}
