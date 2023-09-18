// <copyright file="SendBulkEnvelopes.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("Eg031")]
    public class SendBulkEnvelopes : EgController
    {
        public SendBulkEnvelopes(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg031";

        [HttpPost]
        [SetViewBag]
        [ValidateAntiForgeryToken]
        public IActionResult SetProfile(RecipientModel signer1, RecipientModel carbonCopy1, RecipientModel signer2, RecipientModel carbonCopy2)
        {
            // Check the minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Confirm successful batch send
                var envelopeIdStamping = "true";
                var emailSubject = "Please sign this document sent from the C# SDK";
                var status = global::ESignature.Examples.SendBulkEnvelopes.GetStatus(
                    signer1.Name,
                    signer1.Email,
                    carbonCopy1.Name,
                    carbonCopy1.Email,
                    signer2.Name,
                    signer2.Email,
                    carbonCopy2.Name,
                    carbonCopy2.Email,
                    accessToken,
                    basePath,
                    accountId,
                    this.Config.DocPdf,
                    envelopeIdStamping,
                    emailSubject);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(status, Formatting.Indented);
            }
            catch (Exception ex)
            {
                this.ViewBag.h1 = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                this.ViewBag.message = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage + $@"Reason: {ex}.";
            }

            return this.View("example_done");
        }
    }
}
