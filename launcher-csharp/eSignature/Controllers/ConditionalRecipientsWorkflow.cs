// <copyright file="ConditionalRecipientsWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg034")]
    public class ConditionalRecipientsWorkflow : EgController
    {
        public ConditionalRecipientsWorkflow(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg034";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(RecipientModel recipient1, RecipientModel conditionalRecipient1, RecipientModel conditionalRecipient2)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);

            if (!tokenOk)
            {
                // We could store the parameters of the requested operation
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after
                // authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            EnvelopeSummary results;

            try
            {
                // Call the eSignature API
                results = global::ESignature.Examples.ConditionalRecipientsWorkflow.SendEnvelope(
                    accessToken,
                    basePath,
                    accountId,
                    recipient1.Email,
                    recipient1.Name,
                    conditionalRecipient1.Email,
                    conditionalRecipient1.Name,
                    conditionalRecipient2.Email,
                    conditionalRecipient2.Name);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    this.ViewBag.errorMessage = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }
                else
                {
                    this.ViewBag.errorMessage = apiException.Message;
                }

                return this.View("Error");
            }

            // Process results
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, results.EnvelopeId);
            return this.View("example_done");
        }
    }
}