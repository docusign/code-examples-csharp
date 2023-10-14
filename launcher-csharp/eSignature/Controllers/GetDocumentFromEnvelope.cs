// <copyright file="GetDocumentFromEnvelope.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg007")]
    public class GetDocumentFromEnvelope : EgController
    {
        public GetDocumentFromEnvelope(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg007";

        [HttpPost]
        [SetViewBag]
        public ActionResult Create(string docSelect)
        {
            // Data for this method
            // docSelect -- argument
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            var envelopeId = this.RequestItemsService.EnvelopeId;

            // documents data for the envelope. See example EG006
            List<global::ESignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem> documents =
                this.RequestItemsService.EnvelopeDocuments.Documents.Select(docItems =>
                    new global::ESignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem { DocumentId = docItems.DocumentId, Name = docItems.Name, Type = docItems.Type }).ToList();

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

            // Call the Examples API method to download the specified document from the envelope
            var result = global::ESignature.Examples.GetDocumentFromEnvelope.DownloadDocument(
                accessToken,
                basePath,
                accountId,
                envelopeId,
                (List<global::ESignature.Examples.GetDocumentFromEnvelope.EnvelopeDocItem>)documents,
                docSelect);
            return this.File(result.Item1, result.Item2, result.Item3);
        }
    }
}