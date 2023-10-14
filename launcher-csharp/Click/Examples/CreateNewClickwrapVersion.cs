// <copyright file="CreateNewClickwrapVersion.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class CreateNewClickwrapVersion
    {
        /// <summary>
        /// Creates a new clickwrap version
        /// </summary>
        /// <param name="clickwrapId">The id of clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The summary response of a newly created clickwrap version</returns>
        public static ClickwrapVersionSummaryResponse Create(string clickwrapId, string basePath, string accessToken, string accountId, string clickwrapName)
        {
            //ds-snippet-start:Click3Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:Click3Step2

            //ds-snippet-start:Click3Step3
            var clickwrapRequest = BuildUpdateClickwrapVersionRequest(clickwrapName);
            //ds-snippet-end:Click3Step3

            //ds-snippet-start:Click3Step4
            return clickAccountApi.CreateClickwrapVersion(accountId, clickwrapId, clickwrapRequest);
            //ds-snippet-end:Click3Step4
        }

        //ds-snippet-start:Click3Step3
        private static ClickwrapRequest BuildUpdateClickwrapVersionRequest(string clickwrapName)
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Name = $"{clickwrapName}",
                DisplaySettings = new DisplaySettings()
                {
                    DisplayName = $"{clickwrapName}",
                    MustRead = true,
                    RequireAccept = false,
                    Downloadable = false,
                    SendToEmail = false,
                    ConsentButtonText = "I Agree",
                    Format = "modal",
                    DocumentDisplay = "document",
                },
                Documents = new List<Document>()
                {
                    new Document()
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes("Terms_of_service.pdf")),
                        DocumentName = "Terms of Service",
                        FileExtension = "pdf",
                        Order = 0,
                    },
                },
                Status = "active",
                RequireReacceptance = true,
            };

            return clickwrapRequest;
        }

        //ds-snippet-end:Click3Step3
    }
}
