using System.Collections.Generic;

namespace DocuSign.Click.Examples
{
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class EmbedClickwrap
    {
        /// <summary>
        /// Create a unique agreement URL for this clickwrap
        /// </summary>
        /// <param name="clickwrapId"></param>
        /// <param name="fullName"></param>
        /// <param name="email"></param>
        /// <param name="company"></param>
        /// <param name="title"></param>
        /// <param name="date"></param>
        /// <param name="basePath"></param>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static UserAgreementResponse CreateHasAgreed(string clickwrapId, string fullName, string email, string company, string title, string date, string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            var userAgreementRequest = BuildUpdateClickwrapHasAgreedRequest(fullName, email, company, title, date);

            return clickAccountApi.CreateHasAgreed(accountId, clickwrapId, userAgreementRequest);
        }

        private static UserAgreementRequest BuildUpdateClickwrapHasAgreedRequest(string fullName, string email, string company, string title, string date)
        {
            var userAgreementRequest = new UserAgreementRequest { DocumentData = new Dictionary<string, string>() };
            userAgreementRequest.DocumentData.Add("fullName", fullName);
            userAgreementRequest.DocumentData.Add("email", email);
            userAgreementRequest.DocumentData.Add("company", company);
            userAgreementRequest.DocumentData.Add("title", title);
            userAgreementRequest.DocumentData.Add("date", date);

            userAgreementRequest.ClientUserId = email;

            return userAgreementRequest;
        }

        /// <summary>
        /// Gets a list of active clickwraps
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static ClickwrapVersionsResponse GetActiveClickwraps(string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);
            var options = new AccountsApi.GetClickwrapsOptions();
            options.status = "active";

            return clickAccountApi.GetClickwraps(accountId, options);
        }
    }
}
