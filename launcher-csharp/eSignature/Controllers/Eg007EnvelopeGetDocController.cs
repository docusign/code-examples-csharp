using System.Collections.Generic;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using System.Linq;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg007")]
    public class Eg007EnvelopeGetDocController : EgController
    {
        public Eg007EnvelopeGetDocController(DSConfiguration config, IRequestItemsService requestItemsService) 
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
            List<GetDocumentFromEnvelope.EnvelopeDocItem> documents =
                RequestItemsService.EnvelopeDocuments.Documents.Select(docItems => 
                    new GetDocumentFromEnvelope.EnvelopeDocItem{ DocumentId = docItems.DocumentId, Name = docItems.Name, Type = docItems.Type }).ToList();

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
            var result = GetDocumentFromEnvelope.DownloadDocument(accessToken, basePath, accountId,
                envelopeId, documents, docSelect);
            return File(result.Item1, result.Item2, result.Item3);
        }
    }
}