﻿// <copyright file="HomeController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Monitor.Controllers
{
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("Monitor")]
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
                return this.Redirect($"/{egName}");
            }

            this.ViewBag.APITexts = this.LauncherTexts.ManifestStructure.Groups;
            return this.View();
        }
    }
}