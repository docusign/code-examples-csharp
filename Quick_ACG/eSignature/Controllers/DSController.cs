using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DocuSign.CodeExamples.Controllers
{
    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private IRequestItemsService _requestItemsService;

        private IConfiguration Configuration { get; }

        public AccountController(IRequestItemsService requestItemsService, IConfiguration configuration)
        {
            this._requestItemsService = requestItemsService;
            this.Configuration = configuration;
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

        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            _requestItemsService.Logout();
            return LocalRedirect("/?egName=home");
        }
    }
}
