using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Claims;

namespace eg_03_csharp_auth_code_grant_core.Common
{
    public class LocalsFilter : IActionFilter
    {
        DSConfiguration Config { get; }

        private readonly IRequestItemsService _requestItemsService;
        private IMemoryCache _cache;

        public LocalsFilter(DSConfiguration config, IRequestItemsService requestItemsService, IMemoryCache cache)
        {
            Config = config;
            _cache = cache;
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
            locals.DsConfig = Config;
            locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
            locals.Messages = "";
            locals.Json = null;
            locals.User = null;
            viewBag.Locals = locals;
            viewBag.showDoc = Config.documentation != null;


            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null && !identity.IsAuthenticated)
            {
                locals.Session = new Session();
                return;
            }


            locals.User = httpContext.Session.GetObjectFromJson<User>("user");

            if (locals.User == null)
            {
                locals.User = new User
                {
                    Name = identity.FindFirst(x => x.Type.Equals(ClaimTypes.Name)).Value,
                    AccessToken = identity.FindFirst(x => x.Type.Equals("access_token")).Value,
                    RefreshToken = identity.FindFirst(x => x.Type.Equals("refresh_token")).Value,
                    ExpireIn = DateTime.Parse(identity.FindFirst(x => x.Type.Equals("expires_in")).Value)
                };
                _requestItemsService.User = locals.User;                
            }
            if (locals.Session == null)
            {
                locals.Session = new Session
                {
                    AccountId = identity.FindFirst(x => x.Type.Equals("account_id")).Value,
                    AccountName = identity.FindFirst(x => x.Type.Equals("account_name")).Value,
                    BasePath = identity.FindFirst(x => x.Type.Equals("base_uri")).Value
                };

                _requestItemsService.Session = locals.Session;                
            }        
        }
    }
}
