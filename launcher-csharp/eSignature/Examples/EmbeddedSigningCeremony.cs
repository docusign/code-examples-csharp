using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    /// <summary>
    /// Used to generate an envelope and allow user to sign it directly from the app without having to open an email.
    /// </summary>
    public static class EmbeddedSigningCeremony
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
        public static (string, string) SendEnvelopeForEmbeddedSigning(string signerEmail, string signerName, string signerClientId,
            string accessToken, string basePath, string accountId, string docPdf, string returnUrl, string pingUrl = null)
        {
            // Step 1. Create the envelope definition
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, signerClientId, docPdf);

            // Step 2. Call DocuSign to create the envelope                   
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelope);
            string envelopeId = results.EnvelopeId;

            // Step 3. create the recipient view, the Signing Ceremony
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, returnUrl, signerClientId, pingUrl);
            // call the CreateRecipientView API
            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            // Step 4. Redirect the user to the Signing Ceremony
            // Don't use an iFrame!
            // State can be stored/recovered using the framework's session or a
            // query parameter on the returnUrl (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;
            // returning both the envelopeId as well as the url to be used for embedded signing
            return (envelopeId, redirectUrl);
        }

        private static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global


            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = returnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's
            // authentication, you can include authenticate steps from DocuSign.
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope.
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // Signing Ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app,
            // NOTE: The pings will only be sent if the pingUrl is an https address
            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600"; // seconds
                viewRequest.PingUrl = pingUrl; // optional setting
            }

            return viewRequest;
        }

        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string signerClientId, string docPdf)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // signerClientId -- class global
            // Config.docPdf


            byte[] buffer = System.IO.File.ReadAllBytes(docPdf);

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
            envelopeDefinition.EmailSubject = "Please sign this document";
            Document doc1 = new Document();

            String doc1b64 = Convert.ToBase64String(buffer);

            doc1.DocumentBase64 = doc1b64;
            doc1.Name = "Lorem Ipsum"; // can be different from actual file name
            doc1.FileExtension = "pdf";
            doc1.DocumentId = "1";

            // The order in the docs array determines the order in the envelope
            envelopeDefinition.Documents = new List<Document> { doc1 };

            // Create a signer recipient to sign the document, identified by name and email
            // We set the clientUserId to enable embedded signing for the recipient
            // We're setting the parameters via the object creation

            // create a signer object, in this case for embedded signing
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1"
            };
            // create a radioGroup for the radio buttons
            RadioGroup radioGroup = new RadioGroup
            {
                GroupName = "RadioGroup1",
                DocumentId = "1",
                Radios = new List<Radio>()

            };
            // add the two radio buttons to the radio button group. The first one is defaulted by setting Selected to be "true"
            radioGroup.Radios.Add(new Radio { PageNumber = "1", Value = "radio 1", XPosition = "200", YPosition = "100", Selected = "true" });
            radioGroup.Radios.Add(new Radio { PageNumber = "1", Value = "radio 2", XPosition = "200", YPosition = "120", Selected = "false" });

            // create two SignHer tabs, each is conditional on each of the radio buttons based on their individual values
            SignHere signHere1 = new SignHere
            {
                DocumentId = "1",
                PageNumber = "1",
                XPosition = "100",
                YPosition = "400",
                ConditionalParentLabel = "RadioGroup1",
                ConditionalParentValue = "radio 1"
                
            };
            SignHere signHere2 = new SignHere
            {
                DocumentId = "1",
                PageNumber = "1",
                XPosition = "400",
                YPosition = "400",
                ConditionalParentLabel = "RadioGroup1",
                ConditionalParentValue = "radio 2"
            };
            // add all the tabs to the singer object
            Tabs signer1Tabs = new Tabs
            {
                RadioGroupTabs = new List<RadioGroup> { radioGroup } ,
                SignHereTabs = new List<SignHere> { signHere1, signHere2 }
            };
            signer1.Tabs = signer1Tabs;
            // Add the recipient to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };
            // add recipients to envelope definitions, need to also add documents
            envelopeDefinition.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }
    }
}
