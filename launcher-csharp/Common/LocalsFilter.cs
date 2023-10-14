// <copyright file="LocalsFilter.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Common
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;

    public class LocalsFilter : IActionFilter
    {
        private readonly IRequestItemsService requestItemsService;

        private readonly IConfiguration configuration;

        public LocalsFilter(
            DsConfiguration docuSignConfiguration,
            IRequestItemsService requestItemsService,
            IConfiguration configuration)
        {
            this.DocuSignConfiguration = docuSignConfiguration;
            this.configuration = configuration;
            this.requestItemsService = requestItemsService;
        }

        public DsConfiguration DocuSignConfiguration { get; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Controller controller = context.Controller as Controller;

            if (controller == null)
            {
                return;
            }

            var viewBag = controller.ViewBag;
            var httpContext = context.HttpContext;

            var locals = httpContext.Session.GetObjectFromJson<Locals>("locals") ?? new Locals();
            locals.DsConfig = this.DocuSignConfiguration;
            locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
            locals.Messages = string.Empty;
            locals.Json = null;
            locals.User = null;
            viewBag.Locals = locals;
            viewBag.showDoc = this.DocuSignConfiguration.Documentation != null;

            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null && !identity.IsAuthenticated && (this.requestItemsService.User?.AccessToken == null))
            {
                locals.Session = new Session();
                return;
            }

            locals.User = httpContext.Session.GetObjectFromJson<User>("user");

            if (locals.User == null)
            {
                locals.User = identity.IsAuthenticated
                ? new User
                {
                    Name = identity.FindFirst(x => x.Type.Equals(ClaimTypes.Name)).Value,
                    AccessToken = identity.FindFirst(x => x.Type.Equals("access_token")).Value ?? this.requestItemsService.User.AccessToken,
                    RefreshToken = identity.FindFirst(x => x.Type.Equals("refresh_token"))?.Value,
                    ExpireIn = DateTime.Parse(identity.FindFirst(x => x.Type.Equals("expires_in")).Value ?? identity.Claims.First(x => x.Type.Equals("expires_in")).Value),
                }
                :
                new User
                {
                    Name = this.requestItemsService.User?.Name,
                    AccessToken = this.requestItemsService.User?.AccessToken,
                    RefreshToken = null,
                    ExpireIn = this.requestItemsService.User?.ExpireIn,
                };

                this.requestItemsService.User = locals.User;
            }

            if (locals.Session == null)
            {
                locals.Session = identity.IsAuthenticated
                    ? new Session
                    {
                        AccountId = identity.FindFirst(x => x.Type.Equals("account_id")).Value,
                        AccountName = identity.FindFirst(x => x.Type.Equals("account_name")).Value,
                        BasePath = identity.FindFirst(x => x.Type.Equals("base_uri")).Value,
                        RoomsApiBasePath = this.configuration["DocuSign:RoomsApiEndpoint"],
                        AdminApiBasePath = this.configuration["DocuSign:AdminApiEndpoint"],
                    }
                    :
                    new Session
                    {
                        AccountId = this.requestItemsService.Session.AccountId,
                        AccountName = this.requestItemsService.Session.AccountName,
                        BasePath = this.requestItemsService.Session.BasePath,
                        RoomsApiBasePath = this.configuration["DocuSign:RoomsApiEndpoint"],
                        AdminApiBasePath = this.configuration["DocuSign:AdminApiEndpoint"],
                    };

                this.requestItemsService.Session = locals.Session;
            }
        }
    }
}
