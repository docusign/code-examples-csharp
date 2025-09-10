// <copyright file="Nav001ListAgreementsController.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
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
    [Route("nav001")]
    public class Nav001ListAgreementsController : EgController
    {
        public Nav001ListAgreementsController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Navigator);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "nav001";

        [HttpPost]
        [SetViewBag]
        public async Task<IActionResult> Create(string signerEmail, string signerName)
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

            var agreements = await NavigatorMethods.ListAgreementsWithIamClient(basePath, accessToken, accountId);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(agreements, Formatting.Indented);

            return this.View("example_done");
        }
    }
}
