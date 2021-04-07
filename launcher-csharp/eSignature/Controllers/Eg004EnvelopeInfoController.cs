using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg004")]
    public class Eg004EnvelopeInfoController : EgController
    {
        public Eg004EnvelopeInfoController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Get envelope information";
        }

        public override string EgName => "eg004";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var envelopeId = RequestItemsService.EnvelopeId;

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

            // Call the Examples API method to get the specified envelope's information 
            var results = GetEnvelopeInformation.GetEnvelope(accessToken, basePath, accountId, envelopeId);

            // Process results
            ViewBag.h1 = "Get envelope status results";
            ViewBag.message  = "Results from the Envelopes::get method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}