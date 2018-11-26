using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg005")]
    public class Eg005EnvelopeRecipientsController : EgController
    {
        public Eg005EnvelopeRecipientsController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelope recipients";
        }

        public override string EgName => "eg005";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            var config = new Configuration(new ApiClient(session.BasePath + "/restapi"));
            config.AddDefaultHeader("Authorization", "Bearer " + user.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            ViewBag.h1 = "List envelope recipients result";
            ViewBag.message = "Results from the EnvelopeRecipients::list method:";
            var results = envelopesApi.ListRecipients(RequestItemsService.Session.AccountId, RequestItemsService.EnvelopeId);            
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);

            return View("example_done");
        }
    }
}