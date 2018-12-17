using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg006")]
    public class Eg006EnvelopeDocsController : EgController
    {
        public Eg006EnvelopeDocsController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelope documents";
        }

        public override string EgName => "eg006";

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
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            EnvelopeDocumentsResult result = envelopesApi.ListDocuments(accountId, envelopeId);
            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            List<EnvelopeDocItem> envelopeDocItems = new List<EnvelopeDocItem>();
            envelopeDocItems.Add(new EnvelopeDocItem { Name= "Combined", Type= "content", DocumentId = "combined" });
            envelopeDocItems.Add(new EnvelopeDocItem { Name = "Zip archive", Type = "zip", DocumentId = "archive" });
            
            foreach (EnvelopeDocument doc in result.EnvelopeDocuments)
            {
                envelopeDocItems.Add(new EnvelopeDocItem {
                    DocumentId = doc.DocumentId,
                    Name = doc.DocumentId == "certificate"? "Certificate of completion" : doc.Name,
                    Type = doc.Type
                });
            }

            EnvelopeDocuments envelopeDocuments = new EnvelopeDocuments();
            envelopeDocuments.EnvelopeId = envelopeId;
            envelopeDocuments.Documents = envelopeDocItems;
            RequestItemsService.EnvelopeDocuments = envelopeDocuments;

            ViewBag.envelopeDocuments = envelopeDocuments;
            ViewBag.h1 = "List envelope documents result";
            ViewBag.message = "Results from the EnvelopeDocuments::list method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return View("example_done");
        }
    }
}