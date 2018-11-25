using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
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
            EnvelopesApi envelopesApi = new EnvelopesApi(RequestItemsService.DefaultConfiguration);
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