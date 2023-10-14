// <copyright file="PauseSignatureWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("Eg032")]
    public class PauseSignatureWorkflow : EgController
    {
        public PauseSignatureWorkflow(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg032";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(RecipientModel recipient1, RecipientModel recipient2)
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

            // Call the Examples API method to pause the workflow of signature
            var results = global::ESignature.Examples.PauseSignatureWorkflow.PauseWorkflow(recipient1.Email, recipient1.Name, recipient2.Email, recipient2.Name, accessToken, basePath, accountId);

            // Process results
            this.RequestItemsService.PausedEnvelopeId = results.EnvelopeId;
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return this.View("example_done");
        }
    }
}