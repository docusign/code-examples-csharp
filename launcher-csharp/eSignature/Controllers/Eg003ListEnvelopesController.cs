using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static DocuSign.eSign.Api.EnvelopesApi;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg003")]
    public class Eg003ListEnvelopesController : EgController
    {
        public Eg003ListEnvelopesController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelopes";
        }
   
        public override string EgName => "eg003";

        // ***DS.snippet.0.start
        private EnvelopesInformation DoWork(string accessToken, string basePath, string accountId)
        {
            // Data for this method
            // accessToken
            // basePath
            // accountId

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            ListStatusChangesOptions options = new ListStatusChangesOptions();
            options.fromDate = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");
            // Call the API method:
            EnvelopesInformation results = envelopesApi.ListStatusChanges(accountId, options);
            return results;
        }
        // ***DS.snippet.0.end

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            
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

            // Call the worker
            EnvelopesInformation results = DoWork(accessToken, basePath, accountId);
            // Process results
            ViewBag.h1 = "List envelopes results";
            ViewBag.message = "Results from the Envelopes::listStatusChanges method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results,Formatting.Indented);
            return View("example_done");
        }
    }
}