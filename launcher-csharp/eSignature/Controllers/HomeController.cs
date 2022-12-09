// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace DocuSign.CodeExamples.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using DocuSign.CodeExamples.ESignature.Models;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Configuration;

    public class HomeController : Controller
    {
        private IRequestItemsService RequestItemsService { get; }

        private IConfiguration Configuration { get; }

        private DSConfiguration DsConfiguration { get; }

        private LauncherTexts LauncherTexts { get; }


        private void CheckIfThisIsCFR11Account()
        {
            try
            {
                if (this.RequestItemsService.Session != null)
                {
                    var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
                    var accessToken = this.RequestItemsService.User.AccessToken;
                    var accountId = this.RequestItemsService.Session.AccountId;
                    this.ViewBag.CFRPart11 = global::ESignature.Examples.CFRPart11EmbeddedSending.IsCFRPart11Account(accessToken, basePath, accountId);
                }
            }
            catch
            {
                // ignore this for now, as we're just checking the CFR-11 status
            }
        }

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
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

            if (this.Configuration["quickstart"] == "true")
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    this.Configuration["quickstart"] = "false";
                }

                CheckIfThisIsCFR11Account();
                if (ViewBag.CFRPart11 == true)
                {
                    return this.Redirect("eg041");
                }
                else
                {
                    return this.Redirect("eg001");
                }
            }

            if (this.DsConfiguration.QuickACG == "true")
            {
                CheckIfThisIsCFR11Account();
                if (ViewBag.CFRPart11 == true)
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
                this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.Groups;
                return this.View();
            }

            if (string.IsNullOrEmpty(egName))
            {
                egName = this.RequestItemsService.EgName;
            }

            if (!string.IsNullOrWhiteSpace(egName))
            {
                CheckIfThisIsCFR11Account();
                if (ViewBag.CFRPart11 == true)
                {
                    foreach (var manifestGroup in this.LauncherTexts.ManifestStructure.Groups)
                    {
                        var example = manifestGroup.Examples.Find((example) => example.ExampleNumber == int.Parse(egName.Substring(2)));
                        if (example != null)
                        {
                            // we found the example we're supposed to redirect to, this is a CFR account, if example is NonCFR - show error page
                            if (example.CFREnabled == "NonCFR")
                            {
                                this.ViewBag.errorCode = 0;
                                this.ViewBag.errorMessage = this.LauncherTexts.ManifestStructure.SupportingTexts.CFRError;

                                return this.View("Error");
                            }
                        }
                    }
                }
                this.RequestItemsService.EgName = null;
                return this.Redirect(egName);
            }

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.Groups;
            if (this.RequestItemsService.Session != null)
            {
                CheckIfThisIsCFR11Account();
            }
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

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