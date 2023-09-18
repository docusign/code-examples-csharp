// <copyright file="SetEnvelopeTabValue.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg016")]
    public class SetEnvelopeTabValue : EgController
    {
        private readonly string signerClientId = "1000";

        // Set up the Ping Url, signer client ID, and the return (callback) URL for embedded signing
        private string dsPingUrl;
        private string dsReturnUrl;

        public SetEnvelopeTabValue(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.dsPingUrl = config.AppUrl + "/";
            this.dsReturnUrl = config.AppUrl + "/dsReturn";

            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg016";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Check the token with minimal buffer time
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // The envelope will be sent first to the signer; after it is signed,
            // a copy is sent to the cc person
            //
            // Read files from a local directory
            // The reads could raise an exception if the file is not available!
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Examples API method to create an envelope and set the tab values
                (string envelopeId, string redirectUrl) = global::ESignature.Examples.SetEnvelopeTabValue.CreateEnvelopeAndUpdateTabData(
                    signerEmail,
                    signerName,
                    this.signerClientId,
                    accessToken,
                    basePath,
                    accountId,
                    this.Config.TabsDocx,
                    this.dsReturnUrl,
                    this.dsPingUrl);

                this.RequestItemsService.EnvelopeId = envelopeId;
                return this.Redirect(redirectUrl);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                return this.View("Error");
            }
        }
    }
}