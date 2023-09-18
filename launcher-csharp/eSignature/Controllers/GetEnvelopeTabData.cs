// <copyright file="GetEnvelopeTabData.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Model;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("eg015")]
    public class GetEnvelopeTabData : EgController
    {
        public GetEnvelopeTabData(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg015";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create()
        {
            // Check the token with minimal buffer time
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be restarted
                // automatically. But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after authentication
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var envelopeId = this.RequestItemsService.EnvelopeId;

            // Call the Examples API method to get all tab data from the specified envelope
            EnvelopeFormData results = global::ESignature.Examples.GetEnvelopeTabData.GetEnvelopeFormData(accessToken, basePath, accountId, envelopeId);

            // Process results
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return this.View("example_done");
        }
    }
}