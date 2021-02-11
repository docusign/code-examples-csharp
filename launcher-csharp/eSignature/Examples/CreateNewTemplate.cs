using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class CreateNewTemplate
    {
        private static string templateName = "Example Signer and CC template";

        /// <summary>
        /// Generates a new DocuSign Template based on static information in this class
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>Template name, templateId and a flag to indicate if this is a new template or it exited prior to calling this method</returns>
        public static (bool createdNewTemplate, string templateId, string resultsTemplateName) CreateTemplate(string accessToken, string basePath, string accountId)
        {
            // Step 1. List templates to see if ours exists already
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            TemplatesApi templatesApi = new TemplatesApi(apiClient);
            TemplatesApi.ListTemplatesOptions options = new TemplatesApi.ListTemplatesOptions();
            options.searchText = "Example Signer and CC template";
            EnvelopeTemplateResults results = templatesApi.ListTemplates(accountId, options);

            string templateId;
            string resultsTemplateName;
            bool createdNewTemplate;

            // Step 2. Process results
            if (int.Parse(results.ResultSetSize) > 0)
            {
                // Found the template! Record its id
                templateId = results.EnvelopeTemplates[0].TemplateId;
                resultsTemplateName = results.EnvelopeTemplates[0].Name;
                createdNewTemplate = false;
            }
            else
            {
                // No template! Create one!
                EnvelopeTemplate templateReqObject = MakeTemplate(templateName);

                TemplateSummary template = templatesApi.CreateTemplate(accountId, templateReqObject);

                // Retrieve the new template Name / TemplateId
                EnvelopeTemplateResults templateResults = templatesApi.ListTemplates(accountId, options);
                templateId = templateResults.EnvelopeTemplates[0].TemplateId;
                resultsTemplateName = templateResults.EnvelopeTemplates[0].Name;
                createdNewTemplate = true;

            }

            return (createdNewTemplate: createdNewTemplate,
                templateId: templateId, resultsTemplateName: resultsTemplateName);
        }

        private static EnvelopeTemplate MakeTemplate(string resultsTemplateName)
        {
            // Data for this method
            // resultsTemplateName


            // document 1 (pdf) has tag /sn1/
            //
            // The template has two recipient roles.
            // recipient 1 - signer
            // recipient 2 - cc
            // The template will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read file from a local directory
            // The reads could raise an exception if the file is not available!
            // add the documents
            Document doc = new Document();
            string docB64 = Convert.ToBase64String(System.IO.File.ReadAllBytes("World_Wide_Corp_fields.pdf"));
            doc.DocumentBase64 = docB64;
            doc.Name = "Lorem Ipsum"; // can be different from actual file name
            doc.FileExtension = "pdf";
            doc.DocumentId = "1";

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer();
            signer1.RoleName = "signer";
            signer1.RecipientId = "1";
            signer1.RoutingOrder = "1";
            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            CarbonCopy cc1 = new CarbonCopy();
            cc1.RoleName = "cc";
            cc1.RoutingOrder = "2";
            cc1.RecipientId = "2";
            // Create fields using absolute positioning:
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.XPosition = "191";
            signHere.YPosition = "148";

            Checkbox check1 = new Checkbox();
            check1.DocumentId = "1";
            check1.PageNumber = "1";
            check1.XPosition = "75";
            check1.YPosition = "417";
            check1.TabLabel = "ckAuthorization";

            Checkbox check2 = new Checkbox();
            check2.DocumentId = "1";
            check2.PageNumber = "1";
            check2.XPosition = "75";
            check2.YPosition = "447";
            check2.TabLabel = "ckAuthentication";

            Checkbox check3 = new Checkbox();
            check3.DocumentId = "1";
            check3.PageNumber = "1";
            check3.XPosition = "75";
            check3.YPosition = "478";
            check3.TabLabel = "ckAgreement";

            Checkbox check4 = new Checkbox();
            check4.DocumentId = "1";
            check4.PageNumber = "1";
            check4.XPosition = "75";
            check4.YPosition = "508";
            check4.TabLabel = "ckAcknowledgement";

            List list1 = new List();
            list1.DocumentId = "1";
            list1.PageNumber = "1";
            list1.XPosition = "142";
            list1.YPosition = "291";
            list1.Font = "helvetica";
            list1.FontSize = "size14";
            list1.TabLabel = "list";
            list1.Required = "false";
            list1.ListItems = new List<ListItem>
            {
                new ListItem {Text = "Red", Value = "Red"},
                new ListItem {Text = "Orange", Value = "Orange"},
                new ListItem {Text = "Yellow", Value = "Yellow"},
                new ListItem {Text = "Green", Value = "Green"},
                new ListItem {Text = "Blue", Value = "Blue"},
                new ListItem {Text = "Indigo", Value = "Indigo"},
                new ListItem {Text = "Violet", Value = "Violet"},
            };
            // The SDK can't create a number tab at this time. Bug DCM-2732
            // Until it is fixed, use a text tab instead.
            //   , number = docusign.Number.constructFromObject({
            //         documentId: "1", pageNumber: "1", xPosition: "163", yPosition: "260",
            //         font: "helvetica", fontSize: "size14", tabLabel: "numbersOnly",
            //         height: "23", width: "84", required: "false"})
            Text textInsteadOfNumber = new Text();
            textInsteadOfNumber.DocumentId = "1";
            textInsteadOfNumber.PageNumber = "1";
            textInsteadOfNumber.XPosition = "153";
            textInsteadOfNumber.YPosition = "260";
            textInsteadOfNumber.Font = "helvetica";
            textInsteadOfNumber.FontSize = "size14";
            textInsteadOfNumber.TabLabel = "numbersOnly";
            textInsteadOfNumber.Height = "23";
            textInsteadOfNumber.Width = "84";
            textInsteadOfNumber.Required = "false";

            RadioGroup radioGroup = new RadioGroup();
            radioGroup.DocumentId = "1";
            radioGroup.GroupName = "radio1";

            radioGroup.Radios = new List<Radio>
            {
                new Radio {PageNumber="1", Value="white", XPosition="142", YPosition="384", Required = "false"},
                new Radio {PageNumber="1", Value="red", XPosition="74", YPosition="384", Required = "false"},
                new Radio {PageNumber="1", Value="blue", XPosition="220", YPosition="384", Required = "false"}
            };

            Text text = new Text();
            text.DocumentId = "1";
            text.PageNumber = "1";
            text.XPosition = "153";
            text.YPosition = "230";
            text.Font = "helvetica";
            text.FontSize = "size14";
            text.TabLabel = "text";
            text.Height = "23";
            text.Width = "84";
            text.Required = "false";

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs();
            signer1Tabs.CheckboxTabs = new List<Checkbox>
            {
                check1, check2, check3, check4
            };

            signer1Tabs.ListTabs = new List<List> { list1 };
            // numberTabs: [number],
            signer1Tabs.RadioGroupTabs = new List<RadioGroup> { radioGroup };
            signer1Tabs.SignHereTabs = new List<SignHere> { signHere };
            signer1Tabs.TextTabs = new List<Text> { text, textInsteadOfNumber };

            signer1.Tabs = signer1Tabs;

            // Add the recipients to the env object
            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer1 };
            recipients.CarbonCopies = new List<CarbonCopy> { cc1 };


            // create the overall template definition
            EnvelopeTemplate template = new EnvelopeTemplate();
            // The order in the docs array determines the order in the env
            template.Description = "Example template created via the API";
            template.Name = resultsTemplateName;
            template.Documents = new List<Document> { doc };
            template.EmailSubject = "Please sign this document";
            template.Recipients = recipients;
            template.Status = "created";

            return template;
        }
    }
}
