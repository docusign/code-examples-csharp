using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DocuSign.CodeExamples.Controllers
{
    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private IRequestItemsService _requestItemsService;

        private IConfiguration _configuration { get; }

        public AccountController(IRequestItemsService requestItemsService, IConfiguration configuration)
        {
            this._requestItemsService = requestItemsService;
            this._configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        public IActionResult MustAuthenticate()
        {
            return Login();
        }

        public async System.Threading.Tasks.Task<IActionResult> logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            _requestItemsService.Logout();
            return LocalRedirect("/?egName=home");
        }
    }
}
