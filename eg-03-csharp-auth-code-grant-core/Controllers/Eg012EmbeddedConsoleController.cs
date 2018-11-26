using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg012")]
    public class Eg012EmbeddedConsoleController : EgController
    {
        public Eg012EmbeddedConsoleController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg012";

        [HttpPost]
        public IActionResult Create(string startingView)
        {
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
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            var config = new Configuration(new ApiClient(session.BasePath + "/restapi"));
            config.AddDefaultHeader("Authorization", "Bearer " + user.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            string dsReturnUrl = Config.AppUrl + "/ds-return";
            ConsoleViewRequest viewRequest = MakeConsoleViewRequest(dsReturnUrl, startingView);
            // Step 1. create the NDSE view
            // Call the CreateSenderView API
            // Exceptions will be caught by the calling function
            ViewUrl results = envelopesApi.CreateConsoleView(RequestItemsService.Session.AccountId, viewRequest);            

            Console.WriteLine("NDSE view URL: " + results.Url);

            return Redirect(results.Url);            
        }

        private ConsoleViewRequest MakeConsoleViewRequest(string dsReturnUrl, string startingView)
        {
            ConsoleViewRequest viewRequest = new ConsoleViewRequest();
            // Set the url where you want the recipient to go once they are done
            // with the NDSE. It is usually the case that the
            // user will never "finish" with the NDSE.
            // Assume that control will not be passed back to your app.
            viewRequest.ReturnUrl = dsReturnUrl;

            if ("envelope".Equals(startingView) && RequestItemsService.EnvelopeId != null)
            {
                viewRequest.EnvelopeId = RequestItemsService.EnvelopeId;
            }

            return viewRequest;
        }
    }
}