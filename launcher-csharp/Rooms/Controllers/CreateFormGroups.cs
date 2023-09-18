// <copyright file="CreateFormGroups.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using DocuSign.Rooms.Model;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Rooms")]
    [Route("reg007")]
    public class CreateFormGroups : EgController
    {
        public CreateFormGroups(
            DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "reg007";

        [BindProperty]
        public FormGroupModel FormGroupModel { get; set; }

        [MustAuthenticate]
        [SetViewBag]
        [Route("CreateFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormGroup(FormGroupModel formGroupModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to create form group
                FormGroup formGroup = DocuSign.Rooms.Examples.CreateFormGroups.CreateGroup(basePath, accessToken, accountId, formGroupModel.Name);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, formGroup.FormGroupId.ToString());
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroup, Formatting.Indented);

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.FormGroupModel = new FormGroupModel();
        }
    }
}
