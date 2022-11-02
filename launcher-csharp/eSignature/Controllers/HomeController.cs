// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Diagnostics;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.ESignature.Models;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class HomeController : Controller
    {
        private IRequestItemsService RequestItemsService { get; }

        private IConfiguration Configuration { get; }

        private DSConfiguration DsConfiguration { get; }

        private LauncherTexts LauncherTexts { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts, DSConfiguration dsConfiguration, IConfiguration configuration)
        {
            this.RequestItemsService = requestItemsService;
            this.Configuration = configuration;
            this.DsConfiguration = dsConfiguration;
            this.LauncherTexts = launcherTexts;
        }

        public IActionResult Index(string egName)
        {
            if (this.User.Identity.IsAuthenticated && this.Configuration["FirstLaunch"] == "true")
            {
                this.Configuration["FirstLaunch"] = "false";
                this.Configuration["API"] = ExamplesAPIType.ESignature.ToString();

                return this.Redirect("/ds/Logout");
            }

            this.ViewBag.APIData = JsonConvert.SerializeObject(this.LauncherTexts.ManifestStructure);

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.APIs;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

            if (this.Configuration["quickstart"] == "true")
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    this.Configuration["quickstart"] = "false";
                }

                return this.Redirect("eg001");
            }

            if (this.DsConfiguration.QuickACG == "true")
            {
                return this.Redirect("eg001");
            }

            if (egName == "home")
            {
                return this.View();
            }

            if (string.IsNullOrEmpty(egName))
            {
                egName = this.RequestItemsService.EgName;
            }

            if (!string.IsNullOrWhiteSpace(egName))
            {
                this.RequestItemsService.EgName = null;
                return this.Redirect(egName);
            }

            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            this.ViewBag.APIData = JsonConvert.SerializeObject(this.LauncherTexts.ManifestStructure);
            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.APIs;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [Route("/dsReturn")]
        public IActionResult DsReturn(string state, string @event, string envelopeId)
        {
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            if (this.DsConfiguration.QuickACG == "true")
            {
                return this.Redirect("eg001");
            }

            this.ViewBag.title = "Return from DocuSign";
            this.ViewBag._event = @event;
            this.ViewBag.state = state;
            this.ViewBag.envelopeId = envelopeId;

            return this.View();
        }
    }
}