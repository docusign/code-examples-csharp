// <copyright file="DeleteRestoreEnvelope.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Model;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("Eg045")]
    public class DeleteRestoreEnvelope : EgController
    {
        private const string DeleteFolderId = "recyclebin";

        private const string SentItemsFolderName = "Sent Items";

        public DeleteRestoreEnvelope(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg045";

        /// <summary>
        /// Deletes an envelope by moving it to the recycle bin.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("DeleteEnvelopeAction")]
        [SetViewBag]
        public IActionResult DeleteEnvelopeAction(string envelopeId)
        {
            bool tokenOk = this.CheckToken(3);

            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            this.RequestItemsService.EnvelopeId = envelopeId;

            global::ESignature.Examples.DeleteRestoreEnvelope.MoveEnvelopeToFolder(
                accessToken,
                basePath,
                accountId,
                envelopeId,
                DeleteFolderId);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.ConfirmAdditionalLink = nameof(this.GetRestoreEnvelope);
            this.ViewBag.OnlyConfirmAdditionalLink = true;
            this.ViewBag.message = string.Format(
                this.CodeExampleText.AdditionalPages
                    .FirstOrDefault(x => x.Name.Equals("envelope_is_deleted"))
                    ?.ResultsPageText,
                envelopeId);

            return this.View("example_done");
        }

        /// <summary>
        /// Restores an envelope from the recycle bin back to the Sent Items folder.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("RestoreEnvelopeAction")]
        [SetViewBag]
        public IActionResult RestoreEnvelopeAction(string folderName)
        {
            bool tokenOk = this.CheckToken(3);
            folderName = folderName ?? SentItemsFolderName;

            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            FoldersResponse availableFolders = global::ESignature.Examples.DeleteRestoreEnvelope.GetFolders(
                accessToken,
                basePath,
                accountId);
            Folder folder = global::ESignature.Examples.DeleteRestoreEnvelope.GetFolderIdByName(
                availableFolders.Folders,
                folderName);

            if (folder == null)
            {
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.AdditionalPages[1].ResultsPageText, folderName);
                this.ViewBag.ConfirmAdditionalLink = nameof(this.GetRestoreEnvelope);
                this.ViewBag.OnlyConfirmAdditionalLink = true;

                return this.View("example_done");
            }

            global::ESignature.Examples.DeleteRestoreEnvelope.MoveEnvelopeToFolder(
                accessToken,
                basePath,
                accountId,
                this.RequestItemsService.EnvelopeId,
                folder.FolderId,
                DeleteFolderId);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(
                this.CodeExampleText.ResultsPageText,
                this.RequestItemsService.EnvelopeId,
                folder.Type,
                folderName);

            return this.View("example_done");
        }

        /// <summary>
        /// Displays a page allowing the user to restore a previously deleted envelope.
        /// </summary>
        /// <returns>ActionResult</returns>
        [SetViewBag]
        [HttpGet]
        [Route("GetRestoreEnvelope")]
        public ActionResult GetRestoreEnvelope()
        {
            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            this.ViewBag.EnvelopeId = this.RequestItemsService.EnvelopeId;

            return this.View("restoreEnvelope");
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.ViewBag.EnvelopeId = this.RequestItemsService.EnvelopeId;
        }
    }
}
