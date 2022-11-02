// <copyright file="Eg09AssignFormToFormGroupController.cs" company="DocuSign">
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
    [Route("Reg009")]
    public class AssignFormToFormGroups : EgController
    {
        public AssignFormToFormGroups(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Reg009";

        [BindProperty]
        public FormFormGroupModel FormFormGroupModel { get; set; }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get forms and form groups
                (FormSummaryList forms, FormGroupSummaryList formGroups) =
                    DocuSign.Rooms.Examples.AssignFormToFormGroups.GetFormsAndFormGroups(basePath, accessToken, accountId);

                this.FormFormGroupModel = new FormFormGroupModel { Forms = forms.Forms, FormGroups = formGroups.FormGroups };

                return this.View("Reg009", this);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("AssignFormToFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignFormToFormGroup(FormFormGroupModel formFormGroupModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to assign form to form group
                var formGroupFormToAssign = DocuSign.Rooms.Examples.AssignFormToFormGroups.AssignForm(basePath, accessToken, accountId,
                    formFormGroupModel.FormGroupId, new FormGroupFormToAssign() { FormId = formFormGroupModel.FormId });

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(
                    this.CodeExampleText.ResultsPageText,
                    formGroupFormToAssign.FormId,
                    formFormGroupModel.FormGroupId.ToString());
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroupFormToAssign, Formatting.Indented);

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message.Equals(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck) ?
                    this.CodeExampleText.CustomErrorTexts[0].ErrorMessage : apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.FormFormGroupModel = new FormFormGroupModel();
        }
    }
}
