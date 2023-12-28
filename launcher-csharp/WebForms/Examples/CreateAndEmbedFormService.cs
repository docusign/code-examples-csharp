// <copyright file="CreateAndEmbedFormService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using System.IO;

namespace DocuSign.WebForms.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using DocuSign.WebForms.Api;
    using DocuSign.WebForms.Model;

    public static class CreateAndEmbedFormService
    {
        public static WebFormSummaryList GetForms(DocuSign.WebForms.Client.DocuSignClient docuSignClient, Guid? accountId)
        {
            FormManagementApi formManagementApi = new DocuSign.WebForms.Api.FormManagementApi(docuSignClient);
            var options = new FormManagementApi.ListFormsOptions();
            return formManagementApi.ListForms(accountId, new FormManagementApi.ListFormsOptions());
        }

        public static void AddTemplateIdToForm(string fileLocation, string templateId)
        {
            string targetString = "template-id";

            try
            {
                string fileContent = File.ReadAllText(fileLocation);

                string modifiedContent = fileContent.Replace(targetString, templateId);

                File.WriteAllText(fileLocation, modifiedContent);

                Console.WriteLine("File content has been modified successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static WebFormInstance CreateInstance(DocuSign.WebForms.Client.DocuSignClient docuSignClient, Guid? accountId, string formId)
        {
            FormInstanceManagementApi formManagementApi = new DocuSign.WebForms.Api.FormInstanceManagementApi(docuSignClient);
            var values = new WebFormValues();
            values.Add("PhoneNumber", "555-555-5555");
            values.Add("Yes", new[] { "Yes" });
            values.Add("Company", "Tally");
            values.Add("JobTitle", "Programmer Writer");

            var options = new CreateInstanceRequestBody()
            {
                ClientUserId = "1234-5678-abcd-ijkl",
                FormValues = values,
                ExpirationOffset = 3600,
            };

            return formManagementApi.CreateInstance(accountId, Guid.Parse(formId), options);
        }

        public static List<EnvelopeTemplate> CheckIfTemplateExists(DocuSignClient docuSignClient, string accountId)
        {
            var templatesApi = new TemplatesApi(docuSignClient);
            var listTemplateOptions = new TemplatesApi.ListTemplatesOptions();
            var templateName = "Web Form Example Template";
            listTemplateOptions.searchText = templateName;
            var templates = templatesApi.ListTemplates(accountId, listTemplateOptions);
            return templates.EnvelopeTemplates;
        }

        public static TemplateSummary CreateTemplate(DocuSignClient docuSignClient, string accountId, string documentPdf)
        {
            TemplatesApi templatesApi = new TemplatesApi(docuSignClient);
            var templateName = "Web Form Example Template";

            EnvelopeTemplate templateReqObject = MakeTemplate(templateName, documentPdf);

            TemplateSummary template = templatesApi.CreateTemplate(accountId, templateReqObject);

            return template;
        }

        public static EnvelopeTemplate MakeTemplate(string resultsTemplateName, string documentPdf)
        {
            Document doc = new Document();
            string docB64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(documentPdf));
            doc.DocumentBase64 = docB64;
            doc.Name = "World_Wide_Web_Form";
            doc.FileExtension = "pdf";
            doc.DocumentId = "1";

            Signer signer1 = new Signer();
            signer1.RoleName = "signer";
            signer1.RecipientId = "1";
            signer1.RoutingOrder = "1";

            Checkbox check1 = new Checkbox();
            check1.DocumentId = "1";
            check1.TabLabel = "Yes";
            check1.AnchorString = "/SMS/";
            check1.AnchorUnits = "pixels";
            check1.AnchorXOffset = "20";
            check1.AnchorYOffset = "10";

            SignHere signHere = new SignHere()
            {
                DocumentId = "1",
                TabLabel = "Signature",
                AnchorString = "/SignHere/",
                AnchorUnits = "pixels",
                AnchorXOffset = "20",
                AnchorYOffset = "10",
            };

            Tabs signer1Tabs = new Tabs();
            signer1Tabs.CheckboxTabs = new List<Checkbox>
            {
                check1,
            };

            signer1Tabs.SignHereTabs = new List<SignHere> { signHere };
            signer1Tabs.TextTabs = new List<Text>
            {
                new Text()
                {
                    DocumentId = "1",
                    TabLabel = "FullName",
                    AnchorString = "/FullName/",
                    AnchorUnits = "pixels",
                    AnchorXOffset = "20",
                    AnchorYOffset = "10",
                },
                new Text()
                {
                    DocumentId = "1",
                    TabLabel = "PhoneNumber",
                    AnchorString = "/PhoneNumber/",
                    AnchorUnits = "pixels",
                    AnchorXOffset = "20",
                    AnchorYOffset = "10",
                },
                new Text()
                {
                    DocumentId = "1",
                    TabLabel = "Company",
                    AnchorString = "/Company/",
                    AnchorUnits = "pixels",
                    AnchorXOffset = "20",
                    AnchorYOffset = "10",
                },
                new Text()
                {
                    DocumentId = "1",
                    TabLabel = "JobTitle",
                    AnchorString = "/Title/",
                    AnchorUnits = "pixels",
                    AnchorXOffset = "20",
                    AnchorYOffset = "10",
                },
            };
            signer1Tabs.DateSignedTabs = new List<DateSigned>
            {
                new DateSigned()
                {
                    DocumentId = "1",
                    TabLabel = "DateSigned",
                    AnchorString = "/Date/",
                    AnchorUnits = "pixels",
                    AnchorXOffset = "20",
                    AnchorYOffset = "10",
                },
            };

            signer1.Tabs = signer1Tabs;

            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer1 };

            EnvelopeTemplate template = new EnvelopeTemplate();

            template.Description = "Example template created via the API";
            template.Name = resultsTemplateName;
            template.Shared = "false";
            template.Documents = new List<Document> { doc };
            template.EmailSubject = "Please sign this document";
            template.Recipients = recipients;
            template.Status = "created";

            return template;
        }
    }
}
