using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        public IActionResult MustAuthenticate()
        {
            return View();
        }
        
        public async System.Threading.Tasks.Task<IActionResult> logout()
        {            
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);        
            return LocalRedirect("/");            
        }
    }
}
