using System;
using eg_03_csharp_auth_code_grant_core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace eg_03_csharp_auth_code_grant_core.Rooms
{
    public class MustAuthenticateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestItemsService = context.HttpContext.RequestServices.GetService<IRequestItemsService>();
            var egController = context.Controller as EgController;

            if (egController == null)
            {
                throw new InvalidOperationException("Controller is not of type EgController, attribute is not applicable.");
            }

            bool tokenOk = requestItemsService.CheckToken(3);
            if (!tokenOk)
            {
                requestItemsService.EgName = egController.EgName;
                context.Result = new RedirectResult("/ds/mustAuthenticate");
            }
        }
    }
}
