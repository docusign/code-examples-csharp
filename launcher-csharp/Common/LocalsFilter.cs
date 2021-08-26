using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Claims;

namespace DocuSign.CodeExamples.Common
{
    public class LocalsFilter : IActionFilter
    {
        DSConfiguration DocuSignConfiguration { get; }

        private readonly IRequestItemsService _requestItemsService;
        private IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public LocalsFilter(
            DSConfiguration docuSignConfiguration, 
            IRequestItemsService requestItemsService, 
            IMemoryCache cache, 
            IConfiguration configuration)
        {
            DocuSignConfiguration = docuSignConfiguration;
            _cache = cache;
            _configuration = configuration;
            _requestItemsService = requestItemsService;
        }

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
            locals.DsConfig = DocuSignConfiguration;
            locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
            locals.Messages = "";
            locals.Json = null;
            locals.User = null;
            viewBag.Locals = locals;
            viewBag.showDoc = DocuSignConfiguration.documentation != null;

            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null && !identity.IsAuthenticated && (_requestItemsService.User?.AccessToken == null))
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
                    AccessToken = identity.FindFirst(x => x.Type.Equals("access_token")).Value ?? _requestItemsService.User.AccessToken,
                    RefreshToken = identity.FindFirst(x => x.Type.Equals("refresh_token"))?.Value,
                    ExpireIn = DateTime.Parse(identity.FindFirst(x => x.Type.Equals("expires_in")).Value ?? identity.Claims.First(x => x.Type.Equals("expires_in")).Value)
                }
                :
                new User
                {
                    Name = _requestItemsService.User?.Name,
                    AccessToken = _requestItemsService.User?.AccessToken,
                    RefreshToken = null,
                    ExpireIn = _requestItemsService.User?.ExpireIn
                };

                _requestItemsService.User = locals.User;
            }

            if (locals.Session == null)
            {
                locals.Session = identity.IsAuthenticated
                    ? new Session
                    {
                        AccountId = identity.FindFirst(x => x.Type.Equals("account_id")).Value,
                        AccountName = identity.FindFirst(x => x.Type.Equals("account_name")).Value,
                        BasePath = identity.FindFirst(x => x.Type.Equals("base_uri")).Value,
                        RoomsApiBasePath = _configuration["DocuSign:RoomsApiEndpoint"],
                        AdminApiBasePath = _configuration["DocuSign:AdminApiEndpoint"]
                    }
                    :
                    new Session
                    {
                        AccountId = _requestItemsService.Session.AccountId,
                        AccountName = _requestItemsService.Session.AccountName,
                        BasePath = _requestItemsService.Session.BasePath,
                        RoomsApiBasePath = _configuration["DocuSign:RoomsApiEndpoint"],
                        AdminApiBasePath = _configuration["DocuSign:AdminApiEndpoint"]
                    };

                _requestItemsService.Session = locals.Session;
            }
        }
    }
}
