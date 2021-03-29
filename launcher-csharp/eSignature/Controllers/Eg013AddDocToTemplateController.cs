using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg013")]
    public class Eg013AddDocToTemplateController : EgController
    {
        private string signerClientId = "1000";

        public Eg013AddDocToTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg013";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string item, string quantity)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // item
            // quantity
            // signerClientId -- class global
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            string dsReturnUrl = Config.AppUrl + "/dsReturn";

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

            // Call the Examples API method to create an envelope using both a template and a document
            string redirectUrl = CreateEnvelopeUsingCompositeTemplate.CreateEnvelopeFromCompositeTemplate(signerEmail,
                signerName, ccEmail, ccName, accessToken, basePath, accountId, item, quantity, dsReturnUrl,
                signerClientId, RequestItemsService.TemplateId);

            return Redirect(redirectUrl);
        }
    }
}