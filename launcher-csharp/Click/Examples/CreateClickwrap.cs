// <copyright file="CreateClickwrap.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class CreateClickwrap
    {
        /// <summary>
        /// Creates a new clickwrap
        /// </summary>
        /// <param name="name">The name of new clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The summary response of a newly created clickwrap</returns>
        public static ClickwrapVersionSummaryResponse Create(string name, string basePath, string accessToken, string accountId, string pdfFile)
        {
            //ds-snippet-start:Click1Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Click1Step2
            var clickAccountApi = new AccountsApi(docuSignClient);

            //ds-snippet-start:Click1Step3
            var clickwrapRequest = BuildClickwrapRequest(name, pdfFile);
            //ds-snippet-end:Click1Step3

            //ds-snippet-start:Click1Step4
            var response = clickAccountApi.CreateClickwrapWithHttpInfo(accountId, clickwrapRequest);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:Click1Step4
        }

        //ds-snippet-start:Click1Step3
        public static ClickwrapRequest BuildClickwrapRequest(string name, string pdfFile)
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                DisplaySettings = new DisplaySettings()
                {
                    ConsentButtonText = "I Agree",
                    DisplayName = name,
                    Downloadable = true,
                    Format = "modal",
                    MustRead = true,
                    RequireAccept = true,
                    DocumentDisplay = "document",
                },
                Documents = new List<Document>()
                {
                    new Document()
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(pdfFile)),
                        DocumentName = "Terms of Service",
                        FileExtension = "pdf",
                        Order = 0,
                    },
                },
                Name = name,
                RequireReacceptance = true,
            };

            return clickwrapRequest;
        }

        //ds-snippet-end:Click1Step3
    }
}
