using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg015")]
    public class Eg015EnvelopeTabDataController : EgController
    {
        public Eg015EnvelopeTabDataController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Get Envelope Tab Information";
        }
        public override string EgName => "eg015";

        [HttpPost]
        public IActionResult Create()
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

            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; //represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; //represents your {ACCOUNT_ID}
            var envelopeId = RequestItemsService.EnvelopeId;

            // Step 2: Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);

            // Step 3: Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            EnvelopeFormData results = envelopesApi.GetFormData(accountId, envelopeId);

            ViewBag.h1 = "Get envelope tab data information";
            ViewBag.message = "Results from the Envelopes::get method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}