using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg018")]
    public class Eg018EnvelopeCustomFieldDataController : EgController
    {
        public Eg018EnvelopeCustomFieldDataController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Get custom field data";
        }
        public override string EgName => "eg018";

        [HttpPost]
        public IActionResult Create()
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Rrepresents your {ACCOUNT_ID}
            var envelopeId = RequestItemsService.EnvelopeId;

            // Call the Examples API method to get all the custom fields from the specified envelope
            var results = GetCustomFields.ListAllCustomFieldsForEnvelope(accessToken, basePath, accountId, envelopeId);

            // Process results
            ViewBag.h1 = "Envelope custom field data";
            ViewBag.message = "Results from the EnvelopeCustomFields::list method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}