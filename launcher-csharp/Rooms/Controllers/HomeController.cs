// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System.Diagnostics;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("Rooms")]
    public class HomeController : Controller
    {
        private IRequestItemsService RequestItemsService { get; }

        private LauncherTexts LauncherTexts { get; }

        public HomeController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts)
        {
            this.RequestItemsService = requestItemsService;
            this.LauncherTexts = launcherTexts;
        }

        public IActionResult Index(string egName)
        {
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

            if (string.IsNullOrEmpty(egName))
            {
                egName = this.RequestItemsService.EgName;
            }

            if (!string.IsNullOrWhiteSpace(egName))
            {
                this.RequestItemsService.EgName = null;
                return this.Redirect(egName);
            }

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.Groups;
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
