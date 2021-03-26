using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.Click.Examples
{
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
        public static ClickwrapVersionSummaryResponse Create(string clickwrapId, string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            var clickwrapRequest = BuildUpdateClickwrapVersionRequest();

            return clickAccountApi.CreateClickwrapVersion(accountId, clickwrapId, clickwrapRequest);
        }

        private static ClickwrapRequest BuildUpdateClickwrapVersionRequest()
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Name = "Terms of Service",
                DisplaySettings = new DisplaySettings()
                {
                    DisplayName = "Terms of Service v2",
                    MustRead = true,
                    MustView = false,
                    RequireAccept = false,
                    Downloadable = false,
                    SendToEmail = false,
                    ConsentButtonText = "I Agree",
                    Format = "modal",
                    DocumentDisplay = "document",
                },
                Documents = new List<Document>(){
                    new Document()
                    {
                        DocumentBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes("Terms_of_service.pdf")),
                        DocumentName="Terms of Service",
                        FileExtension="pdf",
                        Order= 0
                    }
                },
                Status = "active",
                RequireReacceptance = true
            };

            return clickwrapRequest;
        }
    }
}
