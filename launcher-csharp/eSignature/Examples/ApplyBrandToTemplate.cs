﻿// <copyright file="ApplyBrandToTemplate.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class ApplyBrandToTemplate
    {
        /// <summary>
        /// Applies a brand to the template
        /// </summary>
        /// <param name="brandId">The brand ID</param>
        /// <param name="templateId">The template ID</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="status">The status of the envelope</param>
        /// <returns>The summary of the envelopes</returns>
        public static EnvelopeSummary CreateEnvelopeFromTemplateWithBrand(string signerEmail, string signerName, string ccEmail, string ccName, string brandId, string templateId, string accessToken, string basePath, string accountId, string status)
        {
            // Construct your API headers
            //ds-snippet-start:eSign30Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            //ds-snippet-end:eSign30Step2

            // Construct your request body
            //ds-snippet-start:eSign30Step3
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                TemplateId = templateId,
                BrandId = brandId,
                TemplateRoles = new List<TemplateRole>
                {
                    new TemplateRole
                    {
                        Name = signerName,
                        Email = signerEmail,
                        RoleName = "signer",
                    },
                    new TemplateRole
                    {
                        Name = ccName,
                        Email = ccEmail,
                        RoleName = "cc",
                    },
                },
                Status = status,
            };
            //ds-snippet-end:eSign30Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign30Step4
            return envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
            //ds-snippet-end:eSign30Step4
        }
    }
}
