// <copyright file="Nav002GetSingleAgreementController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Threading.Tasks;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Navigator")]
    [Route("nav002")]
    public class Nav002GetSingleAgreementController : EgController
    {
        public Nav002GetSingleAgreementController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Navigator);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "nav002";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = this.RequestItemsService.Session.IamBasePath; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var agreements = NavigatorMethods.ListAgreementsWithIamClient(basePath, accessToken, accountId).Result;

            this.ViewBag.Agreements = agreements;

            return this.View("nav002", this);
        }

        [HttpPost]
        [SetViewBag]
        public async Task<IActionResult> Create(string agreementId)
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

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.IamBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;

            var agreement = NavigatorMethods.GetAgreementWithIamClient(basePath, accessToken, accountId, agreementId).Result;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(agreement, Formatting.Indented);

            return this.View("example_done");
        }
    }
}
