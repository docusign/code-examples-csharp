﻿using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg07")]
    public class Eg07CreateFormGroupController : EgController
    {
        

        public Eg07CreateFormGroupController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService) : base(dsConfig, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 7;

        public override string EgName => "Eg07";

        [BindProperty]
        public FormGroupModel FormGroupModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            FormGroupModel = new FormGroupModel();
        }

        [MustAuthenticate]
        [Route("CreateFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormGroup(FormGroupModel formGroupModel)
        {
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to create form group
                FormGroup formGroup = CreateFormGroups.CreateGroup(basePath, accessToken, accountId, formGroupModel.Name);

                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText.Replace("\"{0}\"", formGroup.FormGroupId.ToString());
                ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroup, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}
