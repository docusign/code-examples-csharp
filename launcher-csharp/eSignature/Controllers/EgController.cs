﻿using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Views;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    public abstract class EgController : Controller
    {
        public abstract string EgName { get; }

        protected CodeExampleText codeExampleText { get; set; }

        protected DSConfiguration Config { get; }

        protected LauncherTexts LauncherTexts { get; }

        protected IRequestItemsService RequestItemsService { get; }

        public EgController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
        {
            Config = config;
            RequestItemsService = requestItemsService;
            ViewBag.csrfToken = "";
            LauncherTexts = launcherTexts;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            // Check that the token is valid and will remain valid for awhile to enable the
            // user to fill out the form. If the token is not available, now is the time
            // to have the user authenticate or re-authenticate.
            bool tokenOk = CheckToken();

            if (tokenOk)
            {
                //addSpecialAttributes(model);
                ViewBag.envelopeOk = RequestItemsService.EnvelopeId != null;
                ViewBag.documentsOk = RequestItemsService.EnvelopeDocuments != null;
                ViewBag.documentOptions = RequestItemsService.EnvelopeDocuments?.Documents;
                ViewBag.gatewayOk = Config.GatewayAccountId != null && Config.GatewayAccountId.Length > 25;
                ViewBag.templateOk = RequestItemsService.TemplateId != null;
                ViewBag.source = CreateSourcePath();
                ViewBag.documentation = Config.documentation + EgName;
                ViewBag.showDoc = Config.documentation != null;
                ViewBag.pausedEnvelopeOk = RequestItemsService.PausedEnvelopeId != null;
                InitializeInternal();

                if (Config.QuickACG == "true" && !(this is Eg001EmbeddedSigningController))
                {
                    return Redirect("eg001");
                }

                return View(EgName, this);
            }

            RequestItemsService.EgName = EgName;

            return Redirect("/ds/mustAuthenticate");
        }

        protected virtual void InitializeInternal()
        {
            ViewBag.CodeExampleText = codeExampleText;
        }

        public dynamic CreateSourcePath()
        {
            var uri = Config.githubExampleUrl;
            if (ControllerContext.RouteData.Values["area"] != null)
            {
                uri = $"{uri}/{ControllerContext.RouteData.Values["area"]}";
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
            var groups = LauncherTexts.ManifestStructure.Groups;
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
            return RequestItemsService.CheckToken(bufferMin);
        }
    }
}