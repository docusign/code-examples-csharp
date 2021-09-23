using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using DocuSign.CodeExamples.Common;

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
                returnUrl += "?egName=" + _requestItemsService.EgName;
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
            if (_configuration["ExamplesAPI"] == "Monitor")
            {
                // Monitor API supports JWT only
                return Login("JWT");
            }
            return View();
        }
        
        public async System.Threading.Tasks.Task<IActionResult> logout()
        {            
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            _requestItemsService.Logout();
            _configuration["quickstart"] = "false";
            return LocalRedirect("/?egName=home");            
        }

        /// <summary>
        /// Generates a URL that can be used to obtain consent needed for the JWT Flow
        /// </summary>
        /// <returns>Consent URL</returns>
        private string BuildConsentURL()
        {
            var scopes = "signature impersonation";
            var apiType = Enum.Parse<ExamplesAPIType>(this._configuration["ExamplesAPI"]);
            if (apiType == ExamplesAPIType.Rooms)
            {
                scopes += " dtr.rooms.read dtr.rooms.write dtr.documents.read dtr.documents.write "
                + "dtr.profile.read dtr.profile.write dtr.company.read dtr.company.write room_forms";
            }
            else if (apiType == ExamplesAPIType.Click)
            {
                scopes += " click.manage click.send";
            }
            else if (apiType == ExamplesAPIType.Admin)
            {
                scopes += " user_read user_write organization_read account_read group_read permission_read identity_provider_read domain_read";
            }
            return this._configuration["DocuSign:AuthorizationEndpoint"] + "?response_type=code" +
                "&scope=" + scopes +
                "&client_id=" + this._configuration["DocuSignJWT:ClientId"] + 
                "&redirect_uri=" + this._configuration["DocuSign:AppUrl"] + "/ds/login?authType=JWT";
        }
    }
}
