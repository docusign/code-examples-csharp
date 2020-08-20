using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private IRequestItemsService _requestItemsService;
        private  IConfiguration _configuration { get;  }
        public AccountController(IRequestItemsService requestItemsService, IConfiguration configuration)
        {
            this._requestItemsService = requestItemsService;
            this._configuration = configuration;
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
            if (_configuration["quickstart"] == "true")
            {
                return Login();
            }
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
