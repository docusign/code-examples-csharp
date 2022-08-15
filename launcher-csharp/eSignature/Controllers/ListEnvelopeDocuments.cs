using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eSignature.Examples;
using System.Linq;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg006")]
    public class ListEnvelopeDocuments : EgController
    {
        public ListEnvelopeDocuments(DSConfiguration config, IRequestItemsService requestItemsService) 
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

            // Call the Examples API method to get the list of all documents from the specified envelope
            global::eSignature.Examples.ListEnvelopeDocuments.EnvelopeDocuments envelopeDocuments =
                global::eSignature.Examples.ListEnvelopeDocuments.GetDocuments(accessToken, basePath, accountId, envelopeId);

            // Map the envelopeDocuments object to match the RequestItemsService.EnvelopeDocuments type
            var mappedEnvelopeDocuments = new EnvelopeDocuments
            {
                EnvelopeId = envelopeDocuments.EnvelopeId,
                Documents = envelopeDocuments.Documents.Select(docItem => new EnvelopeDocItem { DocumentId = docItem.DocumentId, Name = docItem.Name, Type = docItem.Type })
                                                       .ToList()
            };

            // Process results
            ViewBag.envelopeDocuments = mappedEnvelopeDocuments;
            ViewBag.h1 = "List envelope documents result";
            ViewBag.message = "Results from the EnvelopeDocuments::list method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(mappedEnvelopeDocuments, Formatting.Indented);

            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            RequestItemsService.EnvelopeDocuments = mappedEnvelopeDocuments;
            // Add PDF Portfolio which is not coming from the GET call
            mappedEnvelopeDocuments.Documents.Add(new EnvelopeDocItem { DocumentId = "portfolio", Name = "PDF Portfolio", Type = "content" });

            return View("example_done");
        }
    }
}