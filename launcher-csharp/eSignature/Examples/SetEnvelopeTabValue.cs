using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class SetEnvelopeTabValue
    {
        /// <summary>
        /// Creates a new envelope and sets the tab values 
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="signerClientId">A unique ID for the embedded signing session for this signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docDocx">String of bytes representing the document (docx)</param>
        /// <param name="returnUrl">URL user will be redirected to after they sign</param>
        /// <param name="pingUrl">URL that DocuSign will be able to ping to incdicate signing session is active</param>
        /// <returns>The envelopeId (GUID) of the resulting Envelope and the URL for the embedded signing</returns>
        public static (string, string) CreateEnvelopeAndUpdateTabData(string signerEmail, string signerName, string signerClientId,
            string accessToken, string basePath, string accountId, string docDocx, string returnUrl, string pingUrl = null)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Create Tabs and CustomFields
            SignHere signHere = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            Text textLegal = new Text
            {
                AnchorString = "/legal/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "legal_name",
                TabLabel = "Legal name",
            };

            Text textFamiliar = new Text
            {
                AnchorString = "/familiar/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "familiar_name",
                TabLabel = "Familiar name"
            };

            // The salary is set both as a readable number in the /salary/ text field,
            // and as a pure number in a custom field ('salary') in the envelope
            int salary = 123000;

            Text textSalary = new Text
            {
                AnchorString = "/salary/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Locked = "true",
                // Convert number to String: 'C2' sets the string 
                // to currency format with two decimal places
                Value = salary.ToString("C2"),
                TabId = "salary",
                TabLabel = "Salary"
            };

            TextCustomField salaryCustomField = new TextCustomField
            {
                Name = "salary",
                Required = "false",
                Show = "true", // Yes, include in the CoC
                Value = salary.ToString()
            };

            CustomFields cf = new CustomFields
            {
                TextCustomFields = new List<TextCustomField> { salaryCustomField }
            };

            // Create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                ClientUserId = signerClientId
            };

            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient/signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere },
                TextTabs = new List<Text> { textLegal, textFamiliar, textSalary }
            };
            signer1.Tabs = signer1Tabs;
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };

            string doc1b64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx));

            // Create document objects, one per document
            Document doc1 = new Document
            {
                DocumentBase64 = doc1b64,
                Name = "Lorem Ipsum", // Can be different from actual file name
                FileExtension = "docx",
                DocumentId = "1"
            };

            // Create the envelope definition
            EnvelopeDefinition envelopeAttributes = new EnvelopeDefinition()
            {
                EnvelopeIdStamping = "true",
                EmailSubject = "Please Sign",
                EmailBlurb = "Sample text for email body",
                Status = "Sent",
                Recipients = recipients,
                CustomFields = cf,
                Documents = new List<Document> { doc1 }
            };

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeAttributes);
            string envelopeId = results.EnvelopeId;

            // Create the View Request
            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the URL where you want the recipient to go once they are done signing;
            // this should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily
            viewRequest.ReturnUrl = returnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's authentication,
            // you can include authentication steps from DocuSign; e.g., SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match the embedded recipient info
            // that we used to create the envelope
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // signing ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the PingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your app
            viewRequest.PingFrequency = "600"; // seconds
                                               // NOTE: The pings will only be sent if the pingUrl is an HTTPS address
            viewRequest.PingUrl = pingUrl; // Optional setting

            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, results.EnvelopeId, viewRequest);
            //***********
            // Don't use an iframe with embedded signing requests!
            //***********
            // State can be stored/recovered using the framework's session or a
            // query parameter on the return URL (see the makeRecipientViewRequest method)
            return (envelopeId, results1.Url);
        }
    }
}
