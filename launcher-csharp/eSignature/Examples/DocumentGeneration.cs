// <copyright file="DocumentGeneration.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public class DocumentGeneration
    {
        private const string DefaultId = "1";

        /// <summary>
        /// Request a signature by email with document generation
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth).</param>
        /// <param name="basePath">BasePath for API calls (URI).</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="candidateEmail">Candidate email.</param>
        /// <param name="candidateName">Candidate name.</param>
        /// <param name="managerName">Manager name.</param>
        /// <param name="jobTitle">Job title.</param>
        /// <param name="salary">Salary for potential candidate.</param>
        /// <param name="rsus">Restricted stock units (RSUs) for potential candidate.</param>
        /// <param name="startDate">Start date of the offer.</param>
        /// <param name="offerDocDocx">String of bytes representing the offer document (pdf).</param>
        /// <returns>EnvelopeId for the new envelope.</returns>
        public static string DocumentGenerationExample(
            string accessToken,
            string basePath,
            string accountId,
            string candidateEmail,
            string candidateName,
            string managerName,
            string jobTitle,
            string salary,
            int rsus,
            DateTime startDate,
            string offerDocDocx)
        {
            DocuSignClient docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            //ds-snippet-start:eSign42Step2
            TemplatesApi templatesApi = new TemplatesApi(docuSignClient);
            var template = templatesApi.CreateTemplateWithHttpInfo(accountId, MakeTemplate());
            template.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            template.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            string templateId = template.Data.TemplateId;
            //ds-snippet-end:eSign42Step2

            //ds-snippet-start:eSign42Step3
            var updateDocumentResponse = templatesApi.UpdateDocumentWithHttpInfo(accountId, templateId, DefaultId, AddDocumentTemplate(offerDocDocx));
            updateDocumentResponse.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            updateDocumentResponse.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign42Step3

            //ds-snippet-start:eSign42Step4
            var createTabsResponse = templatesApi.CreateTabsWithHttpInfo(accountId, templateId, DefaultId, PrepareTabs());
            createTabsResponse.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            createTabsResponse.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign42Step4

            //ds-snippet-start:eSign42Step5
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            var envelopeResponse = envelopesApi.CreateEnvelopeWithHttpInfo(accountId, MakeEnvelope(candidateEmail, candidateName, templateId));
            envelopeResponse.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            envelopeResponse.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            string envelopeId = envelopeResponse.Data.EnvelopeId;
            //ds-snippet-end:eSign42Step5

            //ds-snippet-start:eSign42Step6
            var formFieldsResponse = envelopesApi.GetEnvelopeDocGenFormFieldsWithHttpInfo(accountId, envelopeResponse.Data.EnvelopeId);
            formFieldsResponse.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            formFieldsResponse.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign42Step6

            //ds-snippet-start:eSign42Step7
            DocGenFormFieldRequest preparedFormFields = FormFields(
                formFieldsResponse.Data.DocGenFormFields.FirstOrDefault()?.DocumentId,
                candidateName,
                managerName,
                jobTitle,
                salary,
                rsus.ToString(),
                startDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            var updateEnvelopeDocGenFormFields = envelopesApi.UpdateEnvelopeDocGenFormFieldsWithHttpInfo(
                accountId,
                envelopeId,
                preparedFormFields);
            updateEnvelopeDocGenFormFields.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            updateEnvelopeDocGenFormFields.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign42Step7

            //ds-snippet-start:eSign42Step8
            var envelopeWithDocGen = envelopesApi.UpdateWithHttpInfo(
                accountId,
                envelopeId,
                new Envelope
                {
                    Status = "sent",
                });
            envelopeWithDocGen.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            envelopeWithDocGen.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign42Step8

            return envelopeWithDocGen.Data.EnvelopeId;
        }

        //ds-snippet-start:eSign42Step2
        public static EnvelopeTemplate MakeTemplate()
        {
            Signer signer = new Signer
            {
                RoleName = "signer",
                RecipientId = "1",
                RoutingOrder = "1",
            };

            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer },
            };

            return new EnvelopeTemplate
            {
                Description = "Example template created via the API",
                Name = "Example Template",
                Shared = "false",
                EmailSubject = "Please sign this document",
                Recipients = recipients,
                Status = "created",
            };
        }

        //ds-snippet-end:eSign42Step2

        //ds-snippet-start:eSign42Step4
        public static TemplateTabs PrepareTabs()
        {
            SignHere signHere = new SignHere
            {
                AnchorString = "Employee Signature",
                AnchorUnits = "pixels",
                AnchorXOffset = "5",
                AnchorYOffset = "-22",
            };

            DateSigned dateSignedTabs = new DateSigned
            {
                AnchorString = "Date Signed",
                AnchorUnits = "pixels",
                AnchorYOffset = "-22",
            };

            return new TemplateTabs
            {
                SignHereTabs = new List<SignHere> { signHere },
                DateSignedTabs = new List<DateSigned> { dateSignedTabs },
            };
        }

        //ds-snippet-end:eSign42Step4

        //ds-snippet-start:eSign42Step3
        public static EnvelopeDefinition AddDocumentTemplate(string offerDocumentDocx)
        {
            string documentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(offerDocumentDocx));
            Document document = new Document
            {
                DocumentBase64 = documentBase64,
                Name = "OfferLetterDemo.docx",
                FileExtension = "docx",
                DocumentId = "1",
                Order = "1",
                Pages = "1",
            };

            return new EnvelopeDefinition
            {
                Documents = new List<Document> { document },
            };
        }

        //ds-snippet-end:eSign42Step3

        //ds-snippet-start:eSign42Step5
        public static EnvelopeDefinition MakeEnvelope(string candidateEmail, string candidateName, string templateId)
        {
            TemplateRole signer = new TemplateRole
            {
                Email = candidateEmail,
                Name = candidateName,
                RoleName = "signer",
            };

            return new EnvelopeDefinition
            {
                TemplateRoles = new List<TemplateRole> { signer },
                Status = "created",
                TemplateId = templateId,
            };
        }

        //ds-snippet-end:eSign42Step5

        //ds-snippet-start:eSign42Step7
        public static DocGenFormFieldRequest FormFields(
            string documentId,
            string candidateName,
            string managerName,
            string jobTitle,
            string salary,
            string rsus,
            string startDate)
        {
            return new DocGenFormFieldRequest
            {
                DocGenFormFields = new List<DocGenFormFields>
                {
                    new DocGenFormFields
                    {
                        DocumentId = documentId,
                        DocGenFormFieldList = new List<DocGenFormField>
                        {
                            new DocGenFormField
                            {
                                Name = "Candidate_Name",
                                Value = candidateName,
                            },
                            new DocGenFormField
                            {
                                Name = "Manager_Name",
                                Value = managerName,
                            },
                            new DocGenFormField
                            {
                                Name = "Job_Title",
                                Value = jobTitle,
                            },
                            new DocGenFormField
                            {
                                Name = "Start_Date",
                                Value = startDate,
                            },
                            new DocGenFormField
                            {
                                Name = "Compensation_Package",
                                Type = "TableRow",
                                RowValues = new List<DocGenFormFieldRowValue>
                                {
                                    new DocGenFormFieldRowValue
                                    {
                                        DocGenFormFieldList = new List<DocGenFormField>
                                        {
                                            new DocGenFormField
                                            {
                                                Name = "Compensation_Component",
                                                Value = "Salary",
                                            },
                                            new DocGenFormField
                                            {
                                                Name = "Details",
                                                Value = "$" + salary,
                                            },
                                        },
                                    },
                                    new DocGenFormFieldRowValue
                                    {
                                        DocGenFormFieldList = new List<DocGenFormField>
                                        {
                                            new DocGenFormField
                                            {
                                                Name = "Compensation_Component",
                                                Value = "Bonus",
                                            },
                                            new DocGenFormField
                                            {
                                                Name = "Details",
                                                Value = "20%",
                                            },
                                        },
                                    },
                                    new DocGenFormFieldRowValue
                                    {
                                        DocGenFormFieldList = new List<DocGenFormField>
                                        {
                                            new DocGenFormField
                                            {
                                                Name = "Compensation_Component",
                                                Value = "RSUs",
                                            },
                                            new DocGenFormField
                                            {
                                                Name = "Details",
                                                Value = rsus,
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        //ds-snippet-end:eSign42Step7
    }
}