// <copyright file="RecipientAuthPhone.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg020")]
    public class RecipientAuthPhone : EgController
    {
        public RecipientAuthPhone(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override int EgNumber => 20;

        public override string EgName => "eg020";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string signerCountryCode, string signerPhoneNumber)
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

                // Data for this method:
                // signerEmail
                // signerName
                var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

                // Obtain your OAuth token
                var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
                var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

                // Call the Examples API method to create an envelope and
                // add recipient that is to be authenticated with phone call
                string envelopeId = global::ESignature.Examples.RecipientAuthPhone.CreateEnvelopeWithRecipientUsingPhoneAuth(
                    signerEmail,
                    signerName,
                    accessToken,
                    basePath,
                    accountId,
                    signerCountryCode,
                    signerPhoneNumber,
                    this.Config.DocPdf);

                // Process results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains("IDENTITY_WORKFLOW_INVALID_ID"))
                {
                    // This may indicate that this account is not yet enabled for the new phone auth workflow
                    this.ViewBag.SupportMessage = "Please contact <a target='_blank' href='https://support.docusign.com'>Support</a> to enable recipient phone authentication in your account.";
                }

                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                return this.View("Error");
            }
        }
    }
}