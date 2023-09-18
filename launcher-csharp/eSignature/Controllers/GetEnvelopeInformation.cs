// <copyright file="GetEnvelopeInformation.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("eg004")]
    public class GetEnvelopeInformation : EgController
    {
        public GetEnvelopeInformation(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg004";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            var envelopeId = this.RequestItemsService.EnvelopeId;

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

            // Call the Examples API method to get the specified envelope's information
            var results = global::ESignature.Examples.GetEnvelopeInformation.GetEnvelope(accessToken, basePath, accountId, envelopeId);

            // Process results
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return this.View("example_done");
        }
    }
}