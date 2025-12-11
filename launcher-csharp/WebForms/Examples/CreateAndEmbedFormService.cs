// <copyright file="CreateAndEmbedFormService.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.WebForms.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using DocuSign.WebForms.Api;
    using DocuSign.WebForms.Model;

    public static class CreateAndEmbedFormService
    {
        public static WebFormSummaryList GetForms(Client.DocuSignClient docuSignClient, string accountId)
        {
            //ds-snippet-start:WebForms1Step3
            FormManagementApi formManagementApi = new FormManagementApi(docuSignClient);
            FormManagementApi.ListFormsOptions listFormsOptions = new FormManagementApi.ListFormsOptions();
            listFormsOptions.search = "Web Form Example Template";
            var response = formManagementApi.ListFormsWithHttpInfo(accountId, listFormsOptions);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            return response.Data;
            //ds-snippet-end:WebForms1Step3
        }

        public static void AddTemplateIdToForm(string fileLocation, string templateId)
        {
            string targetString = "template-id";

            try
            {
                string fileContent = File.ReadAllText(fileLocation);
                string modifiedContent = fileContent.Replace(targetString, templateId);

                File.WriteAllText(fileLocation, modifiedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static WebFormInstance CreateInstance(
            Client.DocuSignClient docuSignClient,
            string accountId,
            string formId)
        {
            //ds-snippet-start:WebForms1Step4
            var formValues = new WebFormValues
            {
                { "PhoneNumber", "555-555-5555" },
                { "Yes", new[] { "Yes" } },
                { "Company", "Tally" },
                { "JobTitle", "Programmer Writer" },
            };

            var options = new CreateInstanceRequestBody()
            {
                ClientUserId = "1234-5678-abcd-ijkl",
                FormValues = formValues,
                ExpirationOffset = 24,
            };
            //ds-snippet-end:WebForms1Step4

            //ds-snippet-start:WebForms1Step5
            FormInstanceManagementApi formManagementApi = new FormInstanceManagementApi(docuSignClient);
            var response = formManagementApi.CreateInstanceWithHttpInfo(accountId, formId, options);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            return response.Data;
            //ds-snippet-end:WebForms1Step5
        }

        public static List<EnvelopeTemplate> GetTemplatesByName(
            DocuSignClient docuSignClient,
            string accountId,
            string templateName)
        {
            //ds-snippet-start:WebForms1Step3
            var templatesApi = new TemplatesApi(docuSignClient);
            var listTemplateOptions = new TemplatesApi.ListTemplatesOptions();
            listTemplateOptions.searchText = templateName;

            var templates = templatesApi.ListTemplatesWithHttpInfo(accountId, listTemplateOptions);
            templates.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            templates.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            //ds-snippet-end:WebForms1Step3

            return templates.Data.EnvelopeTemplates;
        }

        public static TemplateSummary CreateTemplate(
            DocuSignClient docuSignClient,
            string accountId,
            string documentPdf,
            string templateName)
        {
            TemplatesApi templatesApi = new TemplatesApi(docuSignClient);

            EnvelopeTemplate templateReqObject = PrepareEnvelopeTemplate(templateName, documentPdf);

            var template = templatesApi.CreateTemplateWithHttpInfo(accountId, templateReqObject);
            template.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            template.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return template.Data;
        }

        public static EnvelopeTemplate PrepareEnvelopeTemplate(string resultsTemplateName, string documentPdf)
        {
            Document document = new Document()
            {
                DocumentBase64 = Convert.ToBase64String(File.ReadAllBytes(documentPdf)),
                Name = "World_Wide_Web_Form",
                FileExtension = "pdf",
                DocumentId = "1",
            };

            Signer signer = new Signer()
            {
                RoleName = "signer",
                RecipientId = "1",
                RoutingOrder = "1",
            };

            Tabs signerTabs = new Tabs()
            {
                CheckboxTabs = new List<Checkbox>
                {
                    new Checkbox()
                    {
                        DocumentId = "1",
                        TabLabel = "Yes",
                        AnchorString = "/SMS/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                },
                SignHereTabs = new List<SignHere>
                {
                    new SignHere()
                    {
                        DocumentId = "1",
                        TabLabel = "Signature",
                        AnchorString = "/SignHere/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "20",
                        AnchorYOffset = "10",
                    },
                },
                TextTabs = new List<Text>
                {
                    new Text()
                    {
                        DocumentId = "1",
                        TabLabel = "FullName",
                        AnchorString = "/FullName/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                    new Text()
                    {
                        DocumentId = "1",
                        TabLabel = "PhoneNumber",
                        AnchorString = "/PhoneNumber/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                    new Text()
                    {
                        DocumentId = "1",
                        TabLabel = "Company",
                        AnchorString = "/Company/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                    new Text()
                    {
                        DocumentId = "1",
                        TabLabel = "JobTitle",
                        AnchorString = "/Title/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                },
                DateSignedTabs = new List<DateSigned>
                {
                    new DateSigned()
                    {
                        DocumentId = "1",
                        TabLabel = "DateSigned",
                        AnchorString = "/Date/",
                        AnchorUnits = "pixels",
                        AnchorXOffset = "0",
                        AnchorYOffset = "0",
                    },
                },
            };

            signer.Tabs = signerTabs;

            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer };

            return new EnvelopeTemplate()
            {
                Description = "Example template created via the eSignature API",
                Name = resultsTemplateName,
                Shared = "false",
                Documents = new List<Document> { document },
                EmailSubject = "Please sign this document",
                Recipients = recipients,
                Status = "created",
            };
        }
    }
}
