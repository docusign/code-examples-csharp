using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Common;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg040")]
    public class Eg040SetDocumentVisibility : EgController
    {
        public Eg040SetDocumentVisibility(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {  
            ViewBag.title = "Set document visibility";
        }

        public override string EgName => "Eg040";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signer1Email, string signer1Name, string signer2Email, string signer2Name, 
            string ccEmail, string ccName)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            string envelopeId = string.Empty;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            try
            {
                envelopeId = SetDocumentVisibility.SendEnvelopeWithEnvelopeVisibility(signer1Email, signer1Name, signer2Email, 
                    signer2Name, ccEmail, ccName, accessToken, basePath, accountId, Config.DocPdf, Config.DocDocx, Config.DocHTML);
            } 
            catch (ApiException apiException) 
            {
                if (apiException.Message.Contains("ACCOUNT_LACKS_PERMISSIONS"))
                {
                    ViewBag.fixingInstructions = "See <a href=\"https://developers.docusign.com/docs/esign-rest-api/how-to/set-document-visibility\">" +
                    "How to set document visibility for envelope recipients</a> in the DocuSign Developer Center " +
                    "for instructions on how to enable document visibility in your developer account.";
                } 
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                
                return View("Error");
            }

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}