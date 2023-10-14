// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Diagnostics;
    using System.Net;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class HomeController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts, DsConfiguration dsConfiguration, IConfiguration configuration)
        {
            this.RequestItemsService = requestItemsService;
            this.Configuration = configuration;
            this.DsConfiguration = dsConfiguration;
            this.LauncherTexts = launcherTexts;
        }

        private IRequestItemsService RequestItemsService { get; }

        private IConfiguration Configuration { get; }

        private DsConfiguration DsConfiguration { get; }

        private LauncherTexts LauncherTexts { get; }

        public IActionResult Index(string egName)
        {
            if (this.User.Identity.IsAuthenticated && this.Configuration["FirstLaunch"] == "true")
            {
                this.Configuration["FirstLaunch"] = "false";
                this.Configuration["API"] = ExamplesApiType.ESignature.ToString();

                return this.Redirect("/ds/Logout");
            }

            this.ViewBag.APIData = JsonConvert.SerializeObject(this.LauncherTexts.ManifestStructure);

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.ApIs;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

            if (this.DsConfiguration.IsLoggedInAfterEg043)
            {
                this.DsConfiguration.IsLoggedInAfterEg043 = false;

                return this.Redirect(this.DsConfiguration.RedirectForEg043);
            }

            if (this.Configuration["quickstart"] == "true")
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    this.Configuration["quickstart"] = "false";
                }

                this.CheckIfThisIsCfr11Account();
                if (this.ViewBag.CFRPart11 == true)
                {
                    return this.Redirect("eg041");
                }
                else
                {
                    return this.Redirect("eg001");
                }
            }

            if (this.DsConfiguration.QuickAcg == "true")
            {
                this.CheckIfThisIsCfr11Account();
                if (this.ViewBag.CFRPart11 == true)
                {
                    return this.Redirect("eg041");
                }
                else
                {
                    return this.Redirect("eg001");
                }
            }

            if (egName == "home")
            {
                this.CheckIfThisIsCfr11Account();
                return this.View();
            }

            if (string.IsNullOrEmpty(egName))
            {
                egName = this.RequestItemsService.EgName;
            }

            if (!string.IsNullOrWhiteSpace(egName))
            {
                this.CheckIfThisIsCfr11Account();
                if (this.ViewBag.CFRPart11 == true)
                {
                    foreach (var apis in this.LauncherTexts.ManifestStructure.ApIs)
                    {
                        foreach (var manifestGroup in apis.Groups)
                        {
                            var example = manifestGroup.Examples.Find((example) =>
                                example.ExampleNumber == int.Parse(egName.Substring(2)));
                            if (example != null)
                            {
                                // we found the example we're supposed to redirect to, this is a CFR account, if example is NonCFR - show error page
                                if (example.CfrEnabled == "NonCFR")
                                {
                                    this.ViewBag.errorCode = 0;
                                    this.ViewBag.errorMessage =
                                        this.LauncherTexts.ManifestStructure.SupportingTexts.CfrError;

                                    return this.View("Error");
                                }
                            }
                        }
                    }
                }

                this.RequestItemsService.EgName = null;
                return this.Redirect(egName);
            }

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.ApIs;
            if (this.RequestItemsService.Session != null)
            {
                this.CheckIfThisIsCfr11Account();
            }

            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            this.ViewBag.APIData = JsonConvert.SerializeObject(this.LauncherTexts.ManifestStructure);
            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.ApIs;

            if (this.Configuration["ErrorMessage"] != null)
            {
                this.ViewBag.errorMessage = this.Configuration["ErrorMessage"];
                this.ViewBag.errorCode = HttpStatusCode.FailedDependency;
            }

            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [Route("/dsReturn")]
        public IActionResult DsReturn(string state, string @event, string envelopeId)
        {
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            if (this.DsConfiguration.QuickAcg == "true")
            {
                return this.Redirect("eg001");
            }

            this.ViewBag.title = "Return from DocuSign";
            this.ViewBag._event = @event;
            this.ViewBag.state = state;
            this.ViewBag.envelopeId = envelopeId;

            return this.View();
        }

        private void CheckIfThisIsCfr11Account()
        {
            try
            {
                if (this.RequestItemsService.Session != null && this.RequestItemsService.User != null)
                {
                    var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
                    var accessToken = this.RequestItemsService.User.AccessToken;
                    var accountId = this.RequestItemsService.Session.AccountId;
                    this.ViewBag.CFRPart11 = global::ESignature.Examples.CfrPart11EmbeddedSending.IsCfrPart11Account(accessToken, basePath, accountId);
                }
            }
            catch
            {
                // ignore this for now, as we're just checking the CFR-11 status
            }
        }
    }
}