// <copyright file="CreateRemoteInstanceService.cs" company="Docusign">
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

    public static class CreateRemoteInstanceService
    {
        public static WebFormSummaryList GetFormsByName(string basePath, string accessToken, string accountId, string templateName)
        {
            //ds-snippet-start:WebForms2Step3
            var client = PrepareWebFormsClient(accessToken, basePath);
            FormManagementApi formManagementApi = new FormManagementApi(client);

            FormManagementApi.ListFormsOptions listFormsOptions = new FormManagementApi.ListFormsOptions
            {
                search = templateName,
            };
            var response = formManagementApi.ListFormsWithHttpInfo(accountId, listFormsOptions);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:WebForms2Step3
        }

        public static void AddTemplateIdToForm(string fileLocation, string templateId)
        {
            string targetString = "template-id";
            string fileContent = File.ReadAllText(fileLocation);
            string modifiedContent = fileContent.Replace(targetString, templateId);

            File.WriteAllText(fileLocation, modifiedContent);
        }

        public static WebFormInstance CreateInstance(
            string basePath,
            string accessToken,
            string accountId,
            string formId,
            string signerEmail,
            string signerName)
        {
            var client = PrepareWebFormsClient(accessToken, basePath);

            var formValues = new WebFormValues
            {
                { "PhoneNumber", "555-555-5555" },
                { "Yes", new[] { "Yes" } },
                { "Company", "Tally" },
                { "JobTitle", "Programmer Writer" },
            };

            //ds-snippet-start:WebForms2Step4
            var requestBody = new CreateInstanceRequestBody()
            {
                SendOption = SendOption.Now,
                FormValues = formValues,
                Recipients = new List<CreateInstanceRequestBodyRecipients>
                {
                    new CreateInstanceRequestBodyRecipients
                    {
                        Email = signerEmail,
                        Name = signerName,
                        RoleName = "signer",
                    },
                },
            };
            //ds-snippet-end:WebForms2Step4

            //ds-snippet-start:WebForms2Step5
            FormInstanceManagementApi formManagementApi = new FormInstanceManagementApi(client);
            var response = formManagementApi.CreateInstanceWithHttpInfo(accountId, formId, requestBody);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:WebForms2Step5
        }

        public static List<EnvelopeTemplate> GetTemplatesByName(
            string basePath,
            string accessToken,
            string accountId,
            string templateName)
        {
            var client = PrepareESignClient(accessToken, basePath);
            var templatesApi = new TemplatesApi(client);

            var options = new TemplatesApi.ListTemplatesOptions
            {
                searchText = templateName,
            };

            var templates = templatesApi.ListTemplatesWithHttpInfo(accountId, options);
            templates.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            templates.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return templates.Data.EnvelopeTemplates;
        }

        public static TemplateSummary CreateTemplate(
            string basePath,
            string accessToken,
            string accountId,
            string documentPdf,
            string templateName)
        {
            var client = PrepareESignClient(accessToken, basePath);
            TemplatesApi templatesApi = new TemplatesApi(client);

            EnvelopeTemplate envelopeTemplate = PrepareEnvelopeTemplate(templateName, documentPdf);

            var response = templatesApi.CreateTemplateWithHttpInfo(accountId, envelopeTemplate);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
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

        private static Client.DocuSignClient PrepareWebFormsClient(string accessToken, string basePath)
        {
            //ds-snippet-start:WebForms2Step2
            var client = new Client.DocuSignClient(basePath);
            client.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            return client;
            //ds-snippet-end:WebForms2Step2
        }

        private static DocuSignClient PrepareESignClient(string accessToken, string basePath)
        {
            var client = new DocuSignClient(basePath);
            client.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            return client;
        }
    }
}
