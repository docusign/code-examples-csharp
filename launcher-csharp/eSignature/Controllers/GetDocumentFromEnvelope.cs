using System.Collections.Generic;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using System.Linq;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg007")]
    public class GetDocumentFromEnvelope : EgController
    {
        public GetDocumentFromEnvelope(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Download a document";
        }

        public override string EgName => "eg007";

        [HttpPost]
        public ActionResult Create(string docSelect)
        {
            // Data for this method
            // docSelect -- argument
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var envelopeId = RequestItemsService.EnvelopeId;
            // documents data for the envelope. See example EG006
            List<global::eSignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem> documents =
                RequestItemsService.EnvelopeDocuments.Documents.Select(docItems => 
                    new global::eSignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem{ DocumentId = docItems.DocumentId, Name = docItems.Name, Type = docItems.Type }).ToList();

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

            // Call the Examples API method to download the specified document from the envelope
            var result = global::eSignature.Examples.GetDocumentFromEnvelope.DownloadDocument(accessToken, basePath, accountId,
                envelopeId, (List<global::eSignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem>)documents, docSelect);
            return File(result.Item1, result.Item2, result.Item3);
        }
    }
}