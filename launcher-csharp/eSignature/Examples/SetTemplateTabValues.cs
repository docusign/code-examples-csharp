using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class SetTemplateTabValues
    {
        /// <summary>
        /// Creates a new envelope based on a template as well as updating the tab values in this envelope and generating a signer view
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="signerClientId">A unique ID for the embedded signing session for this signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="templateId">The DocuSign Template ID</param>
        /// <param name="returnUrl">URL user will be redirected to after they sign</param>
        /// <param name="pingUrl">URL that DocuSign will be able to ping to incdicate signing session is active</param>
        /// <returns>The envelopeId (GUID) of the resulting Envelope and the URL for the embedded signing</returns>
        public static (string,string) CreateEnvelopeFromTempalteAndUpdateTabValues(string signerEmail, string signerName, string ccEmail, string ccName, string signerClientId,
            string accessToken, string basePath, string accountId, string templateId, string returnUrl, string pingUrl = null)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Create Tabs and CustomFields
            // Set the values for the fields in the template
            // List item
            List colorPicker = new List
            {
                Value = "green",
                DocumentId = "1",
                PageNumber = "1",
                TabLabel = "list"
            };

            // Checkboxes
            Checkbox ckAuthorization = new Checkbox
            {
                TabLabel = "ckAuthorization",
                Selected = "true"
            };
            Checkbox ckAgreement = new Checkbox
            {
                TabLabel = "ckAgreement",
                Selected = "true"
            };

            RadioGroup radioGroup = new RadioGroup
            {
                GroupName = "radio1",
                // You only need to provide the readio entry for the entry you're selecting
                Radios = new List<Radio> { new Radio { Value = "white", Selected = "true" } }
            };

            Text includedOnTemplate = new Text
            {
                TabLabel = "text",
                Value = "Jabberywocky!"
            };

            // We can also add a new tab (field) to the ones already in the template
            Text addedField = new Text
            {
                DocumentId = "1",
                PageNumber = "1",
                XPosition = "280",
                YPosition = "172",
                Font = "helvetica",
                FontSize = "size14",
                TabLabel = "added text field",
                Height = "23",
                Width = "84",
                Required = "false",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "name"
            };

            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient/signer
            Tabs tabs = new Tabs
            {
                CheckboxTabs = new List<Checkbox> { ckAuthorization, ckAgreement },
                RadioGroupTabs = new List<RadioGroup> { radioGroup },
                TextTabs = new List<Text> { includedOnTemplate, addedField },
                ListTabs = new List<List> { colorPicker }
            };

            // Create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            TemplateRole signer = new TemplateRole
            {
                Email = signerEmail,
                Name = signerName,
                RoleName = "signer",
                ClientUserId = signerClientId, // Change the signer to be embedded
                Tabs = tabs //Set tab values
            };

            TemplateRole cc = new TemplateRole
            {
                Email = ccEmail,
                Name = ccName,
                RoleName = "cc"
            };

            // Create an envelope custom field to save our application's
            // data about the envelope
            TextCustomField customField = new TextCustomField
            {
                Name = "app metadata item",
                Required = "false",
                Show = "true", // Yes, include in the CoC
                Value = "1234567"
            };

            CustomFields cf = new CustomFields
            {
                TextCustomFields = new List<TextCustomField> { customField }
            };

            // Create the envelope definition
            EnvelopeDefinition envelopeAttributes = new EnvelopeDefinition
            {
                // Uses the template ID received from example 08
                TemplateId = templateId,
                Status = "Sent",
                // Add the TemplateRole objects to utilize a pre-defined
                // document and signing/routing order on an envelope.
                // Template role names need to match what is available on
                // the correlated templateID or else an error will occur
                TemplateRoles = new List<TemplateRole> { signer, cc },
                CustomFields = cf
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
