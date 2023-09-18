// <copyright file="EmbedClickwrap.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using System.Collections.Generic;
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class EmbedClickwrap
    {
        /// <summary>
        /// Create a unique agreement URL for this clickwrap
        /// </summary>
        /// <param name="clickwrapId">Unique Identifier (GUID) of the clickwrap</param>
        /// <param name="fullName">Full Name parameter for dynamic content</param>
        /// <param name="email">Email parameter used for both clientUserId as well as email in dynamic content</param>
        /// <param name="company">Company parameter for dynamic content</param>
        /// <param name="title">Title parameter for dynamic content</param>
        /// <param name="date">Date parameter for dynamic content</param>
        /// <param name="basePath">Base path to make API calls</param>
        /// <param name="accessToken">Access Token to make API calls</param>
        /// <param name="accountId">AccountId (GUID) of the account being used to make API calls</param>
        /// <returns>UserAgreemtnRsponse object containing the agreementUrl to be used</returns>
        public static UserAgreementResponse CreateHasAgreed(string clickwrapId, string fullName, string email, string company, string title, string date, string basePath, string accessToken, string accountId)
        {
            //ds-snippet-start:Click6Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:Click6Step2

            //ds-snippet-start:Click6Step4
            var userAgreementRequest = BuildUpdateClickwrapHasAgreedRequest(fullName, email, company, title, date);

            var response = clickAccountApi.CreateHasAgreedWithHttpInfo(accountId, clickwrapId, userAgreementRequest);
            if (response.StatusCode == 201)
            {
                return response.Data;
            }
            else
            {
                // This typically means that the agreement was already agreed
                var errorAlreadyAgreed = new UserAgreementResponse();
                errorAlreadyAgreed.AgreementUrl = "Already Agreed";
                return errorAlreadyAgreed;
            }

            //ds-snippet-end:Click6Step4
        }

        /// <summary>
        /// Gets a list of active clickwraps
        /// </summary>
        /// <param name="basePath">Base path</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="accountId">Account id</param>
        /// <returns>ClickwrapVersionsResponse</returns>
        public static ClickwrapVersionsResponse GetActiveClickwraps(string basePath, string accessToken, string accountId)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            var options = new AccountsApi.GetClickwrapsOptions();
            options.status = "active";

            return clickAccountApi.GetClickwraps(accountId, options);
        }

        private static UserAgreementRequest BuildUpdateClickwrapHasAgreedRequest(string fullName, string email, string company, string title, string date)
        {
            //ds-snippet-start:Click6Step3
            var userAgreementRequest = new UserAgreementRequest { DocumentData = new Dictionary<string, string>() };
            userAgreementRequest.DocumentData.Add("fullName", fullName);
            userAgreementRequest.DocumentData.Add("email", email);
            userAgreementRequest.DocumentData.Add("company", company);
            userAgreementRequest.DocumentData.Add("title", title);
            userAgreementRequest.DocumentData.Add("date", date);

            userAgreementRequest.ClientUserId = email;

            return userAgreementRequest;
            //ds-snippet-end:Click6Step3
        }
    }
}
