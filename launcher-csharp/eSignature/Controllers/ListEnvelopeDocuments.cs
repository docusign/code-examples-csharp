﻿// <copyright file="ListEnvelopeDocuments.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.ESignature.Models;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Model;
    using global::ESignature.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("eg006")]
    public class ListEnvelopeDocuments : EgController
    {
        public ListEnvelopeDocuments(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg006";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            var envelopeId = this.RequestItemsService.EnvelopeId;

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

            // Call the Examples API method to get the list of all documents from the specified envelope
            EnvelopeDocuments envelopeDocuments =
                ListEnvelopeDocumentsHelpers.GetDocuments(accessToken, basePath, accountId, envelopeId);

            // Map the envelopeDocuments object to match the RequestItemsService.EnvelopeDocuments type
            var mappedEnvelopeDocuments = new EnvelopeDocuments
            {
                EnvelopeId = envelopeDocuments.EnvelopeId,
                Documents = envelopeDocuments.Documents.Select(docItem => new EnvelopeDocItem { DocumentId = docItem.DocumentId, Name = docItem.Name, Type = docItem.Type })
                                                       .ToList(),
            };

            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            this.RequestItemsService.EnvelopeDocuments = mappedEnvelopeDocuments;

            // Process results
            this.ViewBag.envelopeDocuments = mappedEnvelopeDocuments;
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(mappedEnvelopeDocuments, Formatting.Indented);

            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            this.RequestItemsService.EnvelopeDocuments = mappedEnvelopeDocuments;

            // Add PDF Portfolio which is not coming from the GET call
            mappedEnvelopeDocuments.Documents.Add(new EnvelopeDocItem { DocumentId = "portfolio", Name = "PDF Portfolio", Type = "content" });

            return this.View("example_done");
        }
    }
}