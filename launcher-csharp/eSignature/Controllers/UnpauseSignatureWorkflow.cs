// <copyright file="UnpauseSignatureWorkflow.cs" company="DocuSign">
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
    [Route("Eg033")]
    public class UnpauseSignatureWorkflow : EgController
    {
        public UnpauseSignatureWorkflow(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg033";

        [HttpPost]
        [SetViewBag]
        public IActionResult Update()
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

            // Call the eSignature API
            var results = global::ESignature.Examples.UnpauseSignatureWorkflow.UnpauseWorkflow(
                accessToken,
                basePath,
                accountId,
                this.RequestItemsService.PausedEnvelopeId);

            // Process results
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return this.View("example_done");
        }
    }
}
