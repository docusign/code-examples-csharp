using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg006")]
    public class Eg006EnvelopeDocsController : EgController
    {
        public Eg006EnvelopeDocsController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelope documents";
        }

        public override string EgName => "eg006";

        // ***DS.snippet.0.start
        private (EnvelopeDocumentsResult results, EnvelopeDocuments envelopeDocuments) DoWork(
            string accessToken, string basePath, string accountId, string envelopeId)
        {
            // Data for this method
            // accessToken
            // basePath
            // accountId
            // envelopeId

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDocumentsResult results = envelopesApi.ListDocuments(accountId, envelopeId);

            // Prepare and save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            List<EnvelopeDocItem> envelopeDocItems = new List<EnvelopeDocItem>
            {
                new EnvelopeDocItem { Name = "Combined", Type = "content", DocumentId = "combined" },
                new EnvelopeDocItem { Name = "Zip archive", Type = "zip", DocumentId = "archive" }
            };

            foreach (EnvelopeDocument doc in results.EnvelopeDocuments)
            {
                envelopeDocItems.Add(new EnvelopeDocItem
                {
                    DocumentId = doc.DocumentId,
                    Name = doc.DocumentId == "certificate" ? "Certificate of completion" : doc.Name,
                    Type = doc.Type
                });
            }

            EnvelopeDocuments envelopeDocuments = new EnvelopeDocuments
            {
                EnvelopeId = envelopeId,
                Documents = envelopeDocItems
            };

            return (results, envelopeDocuments);
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

            (EnvelopeDocumentsResult results, EnvelopeDocuments envelopeDocuments) = 
                DoWork(accessToken, basePath, accountId, envelopeId);

            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            RequestItemsService.EnvelopeDocuments = envelopeDocuments;
        
            ViewBag.envelopeDocuments = envelopeDocuments;
            ViewBag.h1 = "List envelope documents result";
            ViewBag.message = "Results from the EnvelopeDocuments::list method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}