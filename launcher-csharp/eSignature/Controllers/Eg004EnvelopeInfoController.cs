using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        // ***DS.snippet.0.start
        private Envelope DoWork(string accessToken, string basePath, string accountId,
            string envelopeId)
        {
            // Data for this method
            // accessToken
            // basePath
            // accountId
            // envelopeId

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            ViewBag.h1 = "Get envelope status results";
            ViewBag.message = "Results from the Envelopes::get method:";
            Envelope results = envelopesApi.GetEnvelope(accountId, envelopeId);
            return results;
        }
        // ***DS.snippet.0.end


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

            Envelope results = DoWork(accessToken, basePath, accountId, envelopeId);
        
            ViewBag.h1 = "Get envelope status results";
            ViewBag.message  = "Results from the Envelopes::get method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}