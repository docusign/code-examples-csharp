using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg028")]
    public class Eg028CreateBrandController : EgController
    {
        public Eg028CreateBrandController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Create a new brand";
        }

        public override string EgName => "eg028";

        [HttpPost]
        public IActionResult Create(string brandName, string defaultBrandLanguage)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Data for this method
            // signerEmail 
            // signerName
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2. Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Step 3. Construct your request body
            Brand newBrand = new Brand
            {
                BrandName = brandName,
                DefaultBrandLanguage = defaultBrandLanguage
            };

            try
            {
                // Step 4. Call the eSignature REST API
                var accountsApi = new AccountsApi(apiClient);
                var results = accountsApi.CreateBrand(accountId, newBrand);
                ViewBag.h1 = "New brand created";
                ViewBag.message = "The brand has been created!<br />Brand ID:" + results.Brands[0].BrandId + ".";
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                return View("Error");
            }

            return View("example_done");
        }
    }
}