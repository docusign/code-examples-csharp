using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("Eg030")]
    public class ApplyBrandToTemplate : EgController
    {
        private CodeExampleText codeExampleText;

        public ApplyBrandToTemplate(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 30;
        public override string EgName => "Eg030";

        protected override void InitializeInternal()
        {
            // Data for this method
            // signerEmail 
            // signerName
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(apiClient);
            var brands = accountsApi.ListBrands(accountId);
            ViewBag.Brands = brands.Brands;
            var templatesApi = new TemplatesApi(apiClient);
            var templates = templatesApi.ListTemplates(accountId);
            ViewBag.EnvelopeTemplates = templates.EnvelopeTemplates;
        }

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string cCName, string brandId)
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

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the eSignature 
            var results = global::eSignature.Examples.ApplyBrandToTemplate.CreateEnvelopeFromTemplateWithBrand(signerEmail, signerName, ccEmail,
                cCName, brandId, RequestItemsService.TemplateId, accessToken, basePath, accountId, RequestItemsService.Status);

            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = codeExampleText.ResultsPageText + results.EnvelopeId + ".";
            return View("example_done");
        }
    }
}