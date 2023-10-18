// <copyright file="FocusedView.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Org.BouncyCastle.Asn1;
    using static System.Net.WebRequestMethods;

    /// <summary>
    /// Used to generate an envelope and allow user to sign it directly from the app without having to open an email.
    /// </summary>
    public static class FocusedView
    {
        /// <summary>
        /// Creates a new envelope, adds a single document and a signle recipient (signer) and generates a url that is used for embedded signing.
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="signerClientId">A unique ID for the embedded signing session for this signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="returnUrl">URL user will be redirected to after they sign</param>
        /// <param name="pingUrl">URL that DocuSign will be able to ping to incdicate signing session is active</param>
        /// <returns>The envelopeId (GUID) of the resulting Envelope and the URL for the embedded signing</returns>
        public static (string, string) SendEnvelopeWithFocusedView(
            string signerEmail,
            string signerName,
            string signerClientId,
            string accessToken,
            string basePath,
            string accountId,
            string docPdf,
            string returnUrl,
            string pingUrl = null)
        {
            //ds-snippet-start:eSign44Step3
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, signerClientId, docPdf);

            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelope);
            string envelopeId = results.EnvelopeId;
            //ds-snippet-end:eSign44Step3

            //ds-snippet-start:eSign44Step5
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, returnUrl, signerClientId, pingUrl);

            // Call the CreateRecipientView endpoint
            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            // State can be stored/recovered using the framework's session or a
            // query parameter on the returnUrl (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;

            // Returning both the envelopeId as well as the URL to be used for embedded signing
            return (envelopeId, redirectUrl);
            //ds-snippet-end:eSign44Step5
        }

        //ds-snippet-start:eSign44Step4
        public static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {
            // Data for this method
            // signerEmail
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global
            RecipientViewRequest viewRequest = new RecipientViewRequest();

            // Set the URL where you want the recipient to go once they're done signing
            // It should typically be a callback route somewhere in your app
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing session. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed or spoofed very easily.
            viewRequest.ReturnUrl = returnUrl + "?state=123";

            // How does your app verify the user's authentication? Additionally,
            // you can integrate authentication steps from DocuSign alongside
            // your app's own authentication process
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // signing session. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign signing session web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app.
            // NOTE: The pings will only be sent if the pingUrl is an https address
            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600"; // seconds
                viewRequest.PingUrl = pingUrl; // optional setting
            }

            // The FrameAncestors should include the links https://apps-d.docusign.com
            // for demo environments and https://apps.docusign.com for prod environments,
            // and the site where the document should be embedded.
            // The site must have a valid SSL protocol for the embed to work.
            // MessageOrigins includes the demo or prod link and only
            // takes a single string
            viewRequest.FrameAncestors = new List<string> { "https://localhost:44333", "https://apps-d.docusign.com" };
            viewRequest.MessageOrigins = new List<string> { "https://apps-d.docusign.com" };

            return viewRequest;
        }

        //ds-snippet-end:eSign44Step4

        //ds-snippet-start:eSign44Step2
        public static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string signerClientId, string docPdf)
        {
            // Data for this method:
            // signerEmail
            // signerName
            // signerClientId -- class global
            // Config.docPdf
            byte[] buffer = System.IO.File.ReadAllBytes(docPdf);

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
            envelopeDefinition.EmailSubject = "Please sign this document";
            Document doc1 = new Document();

            string doc1b64 = Convert.ToBase64String(buffer);

            doc1.DocumentBase64 = doc1b64;
            doc1.Name = "Lorem Ipsum"; // Can be different from the actual file name
            doc1.FileExtension = "pdf";
            doc1.DocumentId = "3";

            // The order in the docs array determines the order in the envelope
            envelopeDefinition.Documents = new List<Document> { doc1 };

            // Create a signer recipient to sign the document, identified by name and email
            // We set the clientUserId to enable embedded signing for the recipient
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings
            SignHere signHere1 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorXOffset = "10",
                AnchorYOffset = "20",
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipient to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
            };
            envelopeDefinition.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent"
            // To request that the envelope be created as a draft, set to "created"
            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }

        //ds-snippet-end:eSign44Step2
    }
}
