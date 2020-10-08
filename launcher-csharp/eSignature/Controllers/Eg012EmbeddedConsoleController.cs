using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg012")]
    public class Eg012EmbeddedConsoleController : EgController
    {
        public Eg012EmbeddedConsoleController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg012";

        // ***DS.snippet.0.start
        private string DoWork(string accessToken, string basePath,
            string accountId, string startingView, string dsReturnUrl, string envelopeId)
        {
            // Data for this method
            // startingView
            // accessToken
            // basePath 
            // accountId 
            // dsReturnUrl
            // envelopeId
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            ConsoleViewRequest viewRequest = MakeConsoleViewRequest(dsReturnUrl,
                startingView, envelopeId);

            // Step 1. create the NDSE view
            // Call the CreateSenderView API
            // Exceptions will be caught by the calling function
            ViewUrl results = envelopesApi.CreateConsoleView(accountId, viewRequest);
            string redirectUrl = results.Url;
            return redirectUrl;
        }

        private ConsoleViewRequest MakeConsoleViewRequest(string dsReturnUrl, string startingView,
            string envelopeId)
        {
            // Data for this method
            // dsReturnUrl
            // startingView
            // envelopeId

            ConsoleViewRequest viewRequest = new ConsoleViewRequest();
            // Set the url where you want the recipient to go once they are done
            // with the NDSE. It is usually the case that the
            // user will never "finish" with the NDSE.
            // Assume that control will not be passed back to your app.
            viewRequest.ReturnUrl = dsReturnUrl;

            if ("envelope".Equals(startingView) && envelopeId != null)
            {
                viewRequest.EnvelopeId = envelopeId;
            }

            return viewRequest;
        }
        // ***DS.snippet.0.end


        [HttpPost]
        public IActionResult Create(string startingView)
        {
            // Data for this method
            // startingView
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            string dsReturnUrl = Config.AppUrl + "/dsReturn";
            string envelopeId = RequestItemsService.EnvelopeId;

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

            string redirectUrl = DoWork(accessToken, basePath,
                accountId, startingView, dsReturnUrl, envelopeId);

            Console.WriteLine("NDSE view URL: " + redirectUrl);

            return Redirect(redirectUrl);
        }
    }
}