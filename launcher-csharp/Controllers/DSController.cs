using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private readonly IRequestItemsService _requestItemsService;
        public AccountController(IRequestItemsService requestItemService) 
        {
            this._requestItemsService = requestItemService;
        }

        [HttpGet]
        public IActionResult Login(string authType = "CodeGrant", string returnUrl = "/")
        {
            if (authType == "CodeGrant") 
            {
                return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
            }

            this._requestItemsService.UpdateUserFromJWT();

            return LocalRedirect(returnUrl);
        }

        public IActionResult MustAuthenticate()
        {
            return View();
        }
        
        public async System.Threading.Tasks.Task<IActionResult> logout()
        {            
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            _requestItemsService.Logout();
            return LocalRedirect("/");            
        }
    }
}
