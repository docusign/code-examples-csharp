using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.Click.Examples
{
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
        public static ClickwrapVersionSummaryResponse Create(string name, string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            var clickwrapRequest = BuildClickwraprequest(name);

            return clickAccountApi.CreateClickwrap(accountId, clickwrapRequest);
        }

        private static ClickwrapRequest BuildClickwraprequest(string name)
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                DisplaySettings = new DisplaySettings()
                {
                    ConsentButtonText = "I Agree",
                    DisplayName = "Terms of Service",
                    Downloadable = true,
                    Format = "modal",
                    MustRead = true,
                    MustView = true,
                    RequireAccept = true,
                    DocumentDisplay = "document"
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
                Name = name,
                RequireReacceptance = true
            };

            return clickwrapRequest;
        }
    }
}
