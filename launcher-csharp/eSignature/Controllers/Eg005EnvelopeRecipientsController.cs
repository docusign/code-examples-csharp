using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg005")]
    public class Eg005EnvelopeRecipientsController : EgController
    {
        public Eg005EnvelopeRecipientsController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelope recipients";
        }

        public override string EgName => "eg005";

        // ***DS.snippet.0.start
        private Recipients DoWork(string accessToken, string basePath, string accountId,
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
            ViewBag.h1 = "List envelope recipients result";
            ViewBag.message = "Results from the EnvelopeRecipients::list method:";
            Recipients results = envelopesApi.ListRecipients(accountId, envelopeId);
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
            Recipients results = DoWork(accessToken, basePath, accountId, envelopeId);            
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}