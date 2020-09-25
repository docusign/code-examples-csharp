using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DocuSign.CodeExamples.Controllers
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

            try
            {
                this._requestItemsService.UpdateUserFromJWT();
            }
            catch (ApiException apiExp)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (apiExp.Message.Contains("consent_required"))
                {
                    return Redirect(BuildConsentURL());
                }
            }


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
            _configuration["quickstart"] = "false";
            return LocalRedirect("/");            
        }

        /// <summary>
        /// Generates a URL that can be used to obtain consent needed for the JWT Flow
        /// </summary>
        /// <returns>Consent URL</returns>
        private string BuildConsentURL()
        {
            return this._configuration["DocuSign:AuthorizationEndpoint"] + "?response_type=code" +
                "&scope=signature%20impersonation" +
                "&client_id=" + this._configuration["DocuSignJWT:ClientId"] + 
                "&redirect_uri=" + this._configuration["DocuSign:AppUrl"] + "/ds/login?authType=JWT";
        }
    }
}
