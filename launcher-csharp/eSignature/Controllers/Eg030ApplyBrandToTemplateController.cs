using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("Eg030")]
    public class Eg030ApplyBrandToTemplateController : EgController
    {
        public Eg030ApplyBrandToTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Apply a brand to an envelope created from a template";
        }

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
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string cCName, string brandId, string templateId)
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
            var envelopesApi = new EnvelopesApi(apiClient);

            // Step 3. Construct your request body
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                TemplateId = templateId,
                BrandId = brandId,
                TemplateRoles = new List<TemplateRole>
                {
                    new TemplateRole
                    {
                        Name = signerName,
                        Email = signerEmail,
                        RoleName = "signer"
                    },
                    new TemplateRole
                    {
                        Name = cCName,
                        Email = ccEmail,
                        RoleName = "cc"
                    }
                },
                Status = RequestItemsService.Status
            };

            // Step 4. Call the eSignature 
            var results = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + results.EnvelopeId + ".";
            return View("example_done");
        }
    }
}