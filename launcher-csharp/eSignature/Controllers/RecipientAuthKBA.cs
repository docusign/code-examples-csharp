﻿using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg022")]
    public class RecipientAuthKBA : EgController
    {
        public RecipientAuthKBA(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 22;
        public override string EgName => "eg022";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
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

            // Data for this method:
            // signerEmail 
            // signerName            
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the Examples API method to create an envelope and 
            // add recipient that is to be authenticated with KBA
            string envelopeId = global::eSignature.Examples.RecipientAuthKBA.CreateEnvelopeWithRecipientUsingKBAAuth(signerEmail, signerName,
                accessToken, basePath, accountId);

            // Process results
            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = String.Format(codeExampleText.ResultsPageText, envelopeId);
            return View("example_done");
        }
    }
}