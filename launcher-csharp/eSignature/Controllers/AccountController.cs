// <copyright file="AccountController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private IRequestItemsService requestItemsService;

        public AccountController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts, IConfiguration configuration)
        {
            this.requestItemsService = requestItemsService;
            this.Configuration = configuration;
            this.LauncherTexts = launcherTexts;
        }

        private IConfiguration Configuration { get; }

        private LauncherTexts LauncherTexts { get; }

        [HttpGet]
        public IActionResult Login(string authType = "CodeGrant", string returnUrl = "/")
        {
            this.Configuration["FirstLaunch"] = "false";
            this.Configuration["API"] = this.Configuration["APIPlanned"];

            if (this.Configuration["API"] != this.requestItemsService.IdentifyApiOfCodeExample(this.requestItemsService.EgName))
            {
                this.requestItemsService.EgName = string.Empty;
            }

            if (authType == "CodeGrant")
            {
                returnUrl += "?egName=" + this.requestItemsService.EgName;
                return this.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
            }

            try
            {
                this.requestItemsService.UpdateUserFromJwt();
            }
            catch (ApiException apiExp)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (apiExp.Message.Contains("consent_required"))
                {
                    return this.Redirect(this.BuildConsentUrl());
                }
            }

            returnUrl += "?egName=" + this.requestItemsService.EgName;
            return this.Redirect(returnUrl);
        }

        public IActionResult MustAuthenticate()
        {
            var apiTypeBasedOnExample = this.requestItemsService.IdentifyApiOfCodeExample(this.requestItemsService.EgName);

            if (this.Configuration["API"] == null && (ExamplesApiType.ESignature.ToString() == apiTypeBasedOnExample || this.requestItemsService.EgName == null))
            {
                this.Configuration["APIPlanned"] = ExamplesApiType.ESignature.ToString();
            }
            else
            {
                this.Configuration["APIPlanned"] = apiTypeBasedOnExample;
            }

            if (this.Configuration["APIPlanned"] == "Monitor")
            {
                // Monitor API supports JWT only
                return this.Login("JWT");
            }

            if (this.Configuration["quickstart"] == "true")
            {
                return this.Login();
            }

            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            return this.View();
        }

        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(this.HttpContext);
            this.requestItemsService.Logout();
            return this.LocalRedirect("/?egName=home");
        }

        /// <summary>
        /// Generates a URL that can be used to obtain consent needed for the JWT Flow
        /// </summary>
        /// <returns>Consent URL</returns>
        private string BuildConsentUrl()
        {
            var scopes = "signature impersonation";
            var apiType = Enum.Parse<ExamplesApiType>(this.Configuration["API"]);
            if (apiType == ExamplesApiType.Rooms)
            {
                scopes += " dtr.rooms.read dtr.rooms.write dtr.documents.read dtr.documents.write "
                + "dtr.profile.read dtr.profile.write dtr.company.read dtr.company.write room_forms";
            }
            else if (apiType == ExamplesApiType.Click)
            {
                scopes += " click.manage click.send";
            }
            else if (apiType == ExamplesApiType.Admin)
            {
                scopes += " user_read user_write organization_read account_read group_read permission_read identity_provider_read domain_read user_data_redact asset_group_account_read asset_group_account_clone_write asset_group_account_clone_read";
            }

            return this.Configuration["DocuSign:AuthorizationEndpoint"] + "?response_type=code" +
                "&scope=" + scopes +
                "&client_id=" + this.Configuration["DocuSignJWT:ClientId"] +
                "&redirect_uri=" + this.Configuration["DocuSign:AppUrl"] + "/ds/login?authType=JWT";
        }
    }
}
