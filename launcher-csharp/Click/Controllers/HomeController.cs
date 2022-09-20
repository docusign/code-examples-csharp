// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Click.Controllers
{
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("Click")]
    public class HomeController : Controller
    {
        private IRequestItemsService _requestItemsService { get; }

        private LauncherTexts _launcherTexts { get; }

        public HomeController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts)
        {
            this._requestItemsService = requestItemsService;
            this._launcherTexts = launcherTexts;
        }

        public IActionResult Index(string egName)
        {
            this.ViewBag.SupportingTexts = this._launcherTexts.ManifestStructure.SupportingTexts;

            if (string.IsNullOrEmpty(egName))
            {
                egName = this._requestItemsService.EgName;
            }

            if (egName == "home")
            {
                this.ViewBag.APITexts = this._launcherTexts.ManifestStructure.Groups;
                return this.View();
            }

            if (!string.IsNullOrWhiteSpace(egName))
            {
                this._requestItemsService.EgName = null;
                return this.Redirect($"/{egName}");
            }

            this.ViewBag.APITexts = this._launcherTexts.ManifestStructure.Groups;
            return this.View();
        }
    }
}