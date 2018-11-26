using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static DocuSign.eSign.Api.EnvelopesApi;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg003")]
    public class Eg003ListEnvelopesController : EgController
    {
        public Eg003ListEnvelopesController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelopes";
        }
   
        public override string EgName => "eg003";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
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
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            var config = new Configuration(new ApiClient(session.BasePath + "/restapi"));
            config.AddDefaultHeader("Authorization", "Bearer " + user.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            ListStatusChangesOptions options = new ListStatusChangesOptions();
                   
            options.fromDate = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");

            EnvelopesInformation results = envelopesApi.ListStatusChanges(RequestItemsService.Session.AccountId, options);
            
            ViewBag.h1 = "List envelopes results";
            ViewBag.message = "Results from the Envelopes::listStatusChanges method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results,Formatting.Indented);
            return View("example_done");
        }
    }
}