using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg008")]
    public class CreateNewTemplate : EgController
    {
        private CodeExampleText codeExampleText;

        public CreateNewTemplate(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 8;

        public override string EgName => "eg008";

        [HttpPost]
        public IActionResult Create()
        {
            // Data for this method
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Call the Examples API method to create a new DocuSign template
            (bool createdNewTemplate, string templateId, string resultsTemplateName) = global::eSignature.Examples.CreateNewTemplate.CreateTemplate(
                 accessToken, basePath, accountId);

            // Save the templateId
            RequestItemsService.TemplateId = templateId;
            string msg = createdNewTemplate ?
                    "The template has been created!" :
                    "The template already exists in your account.";
            ViewBag.message = msg + "<br/>Template name: " + resultsTemplateName + ", ID " + templateId + ".";

            return View("example_done");
        }
    }
}