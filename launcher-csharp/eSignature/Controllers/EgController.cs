// <copyright file="EgController.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.ESignature.Models;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Views;
    using Microsoft.AspNetCore.Mvc;

    public abstract class EgController : Controller
    {
        public EgController(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
        {
            this.Config = config;
            this.RequestItemsService = requestItemsService;
            this.ViewBag.csrfToken = string.Empty;
            this.LauncherTexts = launcherTexts;
        }

        public LauncherTexts LauncherTexts { get; }

        public abstract string EgName { get; }

        protected CodeExampleText CodeExampleText { get; set; }

        protected DsConfiguration Config { get; }

        protected IRequestItemsService RequestItemsService { get; }

        public bool CheckIfAuthorizationIsNeeded()
        {
            bool tokenOk = this.CheckToken();
            ExamplesApiType previousLoginSchema = Enum.Parse<ExamplesApiType>(this.RequestItemsService.Configuration["API"]);
            ExamplesApiType currentApi = Enum.Parse<ExamplesApiType>(this.RequestItemsService.IdentifyApiOfCodeExample(this.EgName));
            this.RequestItemsService.Configuration["APIPlanned"] = currentApi.ToString();

            if (currentApi == ExamplesApiType.Connect)
            {
                return false;
            }

            if (tokenOk && previousLoginSchema == currentApi)
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            // Check that the token is valid and will remain valid for awhile to enable the
            // user to fill out the form. If the token is not available, now is the time
            // to have the user authenticate or re-authenticate.
            if (this.RequestItemsService.Configuration["API"] == null)
            {
                this.RequestItemsService.Configuration["API"] = ExamplesApiType.ESignature.ToString();
            }

            if (this.CheckIfAuthorizationIsNeeded())
            {
                this.RequestItemsService.EgName = this.EgName;
                this.Response.Redirect("/ds/mustAuthenticate");

                return this.LocalRedirect("/ds/mustAuthenticate");
            }

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

            if (this.Config.QuickAcg == "true" && !(this is EmbeddedSigningCeremony))
            {
                return this.Redirect("eg001");
            }

            return this.View(this.EgName, this);
        }

        public dynamic CreateSourcePath()
        {
            var uri = this.Config.GithubExampleUrl;
            // eg001 is at the top level
            if (this.EgName != "eg001")
            {
                uri = $"{uri}/{this.ControllerContext.RouteData.Values["area"]}";
                return $"{uri}/Examples/{this.GetType().Name}.cs";
            }
            else if (this.EgName == "meg001")
            {
                return "https://github.com/docusign/code-examples-csharp/blob/master/launcher-csharp/Monitor/Examples/GetMonitoringData.cs";
            }
            else if (this.EgName != "eg001")
            {
                // eg001 is at the top level
                uri = $"{uri}/eSignature";
                return $"{uri}/Examples/{this.GetType().Name}.cs";
            }
            else
            {
                return $"{uri}/{this.GetType().Name}.cs";
            }
        }

        protected virtual void InitializeInternal()
        {
            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
        }

        protected CodeExampleText GetExampleText(string exampleName, ExamplesApiType examplesApiType)
        {
            int exampleNumber = int.Parse(Regex.Match(exampleName, @"\d+").Value);
            var groups = this.LauncherTexts.ManifestStructure.ApIs
                .Find(x => x.Name.ToLowerInvariant() == examplesApiType.ToString().ToLowerInvariant())
                .Groups;

            foreach (var group in groups)
            {
                var example = group.Examples.Find((example) => example.ExampleNumber == exampleNumber);

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

        protected void GetHttpInfo(IDictionary<string, string> headers)
        {
            headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
        }
    }
}