using System;
using System.Net;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using eSignature.Examples;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg010")]
    public class CreateEnvelopeWithMultipleDocumentTypes : EgController
    {
        public CreateEnvelopeWithMultipleDocumentTypes(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 10;

        public override string EgName => "eg010";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;

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

            // Call the Examples API method to create the envelope multiple types of documents and send it
            (bool statusOk, string envelopeId, string errorCode, string errorMessage, WebException webEx) =
                global::eSignature.Examples.CreateEnvelopeWithMultipleDocumentTypes.CreateAndSendEnvelope(signerEmail, signerName, ccEmail, ccName,
                    Config.docDocx, Config.docPdf, accessToken, basePath, accountId);

            // Process results
            if (statusOk)
            {
                RequestItemsService.EnvelopeId = envelopeId;
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = String.Format(codeExampleText.ResultsPageHeader, envelopeId);
                return View("example_done");
            }
            else
            {
                ViewBag.errorCode = errorCode;
                ViewBag.errorMessage = errorMessage;
                return View("error");
            }
        }
    }
}