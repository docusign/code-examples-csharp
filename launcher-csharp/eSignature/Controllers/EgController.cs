// <copyright file="EgController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.ESignature.Models;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Views;
    using Microsoft.AspNetCore.Mvc;

    public abstract class EgController : Controller
    {
        public abstract string EgName { get; }

        public abstract int EgNumber { get; }

        protected CodeExampleText CodeExampleText { get; set; }

        protected DSConfiguration Config { get; }

        protected LauncherTexts LauncherTexts { get; }

        protected IRequestItemsService RequestItemsService { get; }

        public EgController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
        {
            this.Config = config;
            this.RequestItemsService = requestItemsService;
            this.ViewBag.csrfToken = string.Empty;
            this.LauncherTexts = launcherTexts;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            // Check that the token is valid and will remain valid for awhile to enable the
            // user to fill out the form. If the token is not available, now is the time
            // to have the user authenticate or re-authenticate.
            bool tokenOk = this.CheckToken();

            if (tokenOk)
            {
                // addSpecialAttributes(model);
                this.ViewBag.envelopeOk = this.RequestItemsService.EnvelopeId != null;
                this.ViewBag.documentsOk = this.RequestItemsService.EnvelopeDocuments != null;
                this.ViewBag.documentOptions = this.RequestItemsService.EnvelopeDocuments?.Documents;
                this.ViewBag.gatewayOk = this.Config.GatewayAccountId != null && this.Config.GatewayAccountId.Length > 25;
                this.ViewBag.templateOk = this.RequestItemsService.TemplateId != null;
                this.ViewBag.source = this.CreateSourcePath();
                this.ViewBag.documentation = this.Config.Documentation + this.EgName;
                this.ViewBag.showDoc = this.Config.Documentation != null;
                this.ViewBag.pausedEnvelopeOk = this.RequestItemsService.PausedEnvelopeId != null;
                this.InitializeInternal();

                if (this.Config.QuickACG == "true" && !(this is Eg001EmbeddedSigningController))
                {
                    return this.Redirect("eg001");
                }

                return this.View(this.EgName, this);
            }

            this.RequestItemsService.EgName = this.EgName;

            return this.Redirect("/ds/mustAuthenticate");
        }

        protected virtual void InitializeInternal()
        {
            this.ViewBag.CodeExampleText = this.CodeExampleText;
        }

        public dynamic CreateSourcePath()
        {
            var uri = this.Config.GithubExampleUrl;
            if (this.ControllerContext.RouteData.Values["area"] != null)
            {
                uri = $"{uri}/{this.ControllerContext.RouteData.Values["area"]}";
                return $"{uri}/Examples/{this.GetType().Name}.cs";
            }
            else if (this.EgName == "monitorExample001")
            {
                return "https://github.com/docusign/code-examples-csharp/blob/master/launcher-csharp/Monitor/Examples/GetMonitoringData.cs";
            }
            else if (this.EgName != "eg001") // eg001 is at the top level
            {
                uri = $"{uri}/eSignature";
                return $"{uri}/Examples/{this.GetType().Name}.cs";
            }
            else
            {
                return $"{uri}/{this.GetType().Name}.cs";
            }
        }

        protected CodeExampleText GetExampleText(int number)
        {
            var groups = this.LauncherTexts.ManifestStructure.Groups;
            foreach (var group in groups)
            {
                var example = group.Examples.Find((example) => example.ExampleNumber == number);

                if (example != null)
                {
                    return example;
                }
            }

            return null;
        }

        protected bool CheckToken(int bufferMin = 60)
        {
            return this.RequestItemsService.CheckToken(bufferMin);
        }
    }
}