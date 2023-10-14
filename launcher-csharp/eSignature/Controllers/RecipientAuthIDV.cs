// <copyright file="RecipientAuthIDV.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg023")]
    public class RecipientAuthIdv : EgController
    {
        public RecipientAuthIdv(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg023";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            try
            {
                // Check the token with minimal buffer time.
                bool tokenOk = this.CheckToken(3);
                if (!tokenOk)
                {
                    // We could store the parameters of the requested operation so it could be
                    // restarted automatically. But since it should be rare to have a token issue
                    // here, we'll make the user re-enter the form data after authentication.
                    this.RequestItemsService.EgName = this.EgName;
                    return this.Redirect("/ds/mustAuthenticate");
                }

                // Data for this method
                // signerEmail
                // signerName
                var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

                // Obtain your OAuth token
                var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
                var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

                // Call the Examples API method to create an envelope and
                // add recipient that is to be authenticated with IDV
                string envelopeId = global::ESignature.Examples.RecipientAuthIdv.CreateEnvelopeWithRecipientUsingIdvAuth(signerEmail, signerName, accessToken, basePath, accountId);

                // Process results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    // This may indicate that this account is not yet enabled for the new phone auth workflow
                    this.ViewBag.SupportMessage = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }

                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }
    }
}