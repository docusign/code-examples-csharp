using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Common;
using System;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg040")]
    public class Eg040SetDocumentVisibility : EgController
    {
        public Eg040SetDocumentVisibility(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
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
                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    ViewBag.fixingInstructions = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                } 
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = String.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return View("example_done");
        }
    }
}