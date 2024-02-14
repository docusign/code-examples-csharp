// <copyright file="DocumentGeneration.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
            DateTime startDate,
            string offerDocDocx)
        {
            DocuSignClient docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            //ds-snippet-start:eSign42Step2
            TemplatesApi templatesApi = new TemplatesApi(docuSignClient);
            TemplateSummary template = templatesApi.CreateTemplate(accountId, MakeTemplate());
            string templateId = template.TemplateId;
            //ds-snippet-end:eSign42Step2

            //ds-snippet-start:eSign42Step3
            templatesApi.UpdateDocument(accountId, templateId, DefaultId, AddDocumentTemplate(offerDocDocx));
            //ds-snippet-end:eSign42Step3

            //ds-snippet-start:eSign42Step4
            templatesApi.CreateTabs(accountId, templateId, DefaultId, PrepareTabs());
            //ds-snippet-end:eSign42Step4

            //ds-snippet-start:eSign42Step5
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary envelope = envelopesApi.CreateEnvelope(accountId, MakeEnvelope(candidateEmail, candidateName, templateId));
            string envelopeId = envelope.EnvelopeId;
            //ds-snippet-end:eSign42Step5

            //ds-snippet-start:eSign42Step6
            DocGenFormFieldResponse formFields = envelopesApi.GetEnvelopeDocGenFormFields(accountId, envelope.EnvelopeId);
            //ds-snippet-end:eSign42Step6

            //ds-snippet-start:eSign42Step7
            DocGenFormFieldRequest preparedFormFields = FormFields(
                formFields.DocGenFormFields.FirstOrDefault()?.DocumentId,
                candidateName,
                managerName,
                jobTitle,
                salary,
                startDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            envelopesApi.UpdateEnvelopeDocGenFormFields(
                accountId,
                envelopeId,
                preparedFormFields);
            //ds-snippet-end:eSign42Step7

            //ds-snippet-start:eSign42Step8
            EnvelopeUpdateSummary envelopeWithDocGen = envelopesApi.Update(
                accountId,
                envelopeId,
                new Envelope
                {
                    Status = "sent",
                });
            //ds-snippet-end:eSign42Step8

            return envelopeWithDocGen.EnvelopeId;
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
                AnchorString = "Date",
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
                                Name = "Salary",
                                Value = salary,
                            },
                            new DocGenFormField
                            {
                                Name = "Start_Date",
                                Value = startDate,
                            },
                        },
                    },
                },
            };
        }

        //ds-snippet-end:eSign42Step7
    }
}