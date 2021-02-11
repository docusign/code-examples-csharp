using System;
using System.Collections.Generic;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg014")]
    public class Eg014CollectPaymentController : EgController
    {
        public Eg014CollectPaymentController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Envelope sent";
        }

        public override string EgName => "eg014";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
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

            // Call the Examples API method to create an envelope with payment processing
            string envelopeId = CollectPaymentInEnvelope.CreateEnvelopeWithPayment(signerEmail, signerName, ccEmail,
                ccName, accessToken, basePath, accountId, RequestItemsService.Status, Config.GatewayAccountId, Config.GatewayName, Config.GatewayDisplayName);

            // Process results
            Console.WriteLine("Envelope was created.EnvelopeId " + envelopeId);
            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br/>Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}
