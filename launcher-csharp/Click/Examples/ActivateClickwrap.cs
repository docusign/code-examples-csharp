using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Model;

namespace DocuSign.Click.Examples
{
    public class ActivateClickwrap
    {
        /// <summary>
        /// Activates a newly created clickwrap
        /// </summary>
        /// <param name="clickwrapId">The Id of clickwrap</param>
        /// <param name="clickwrapVersion">The version of clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The summary response of the activated clickwrap</returns>
        public static ClickwrapVersionSummaryResponse Update(string clickwrapId, string clickwrapVersion, string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            var clickwrapRequest = BuildUpdateClickwrapVersionRequest();

            return clickAccountApi.UpdateClickwrapVersion(accountId, clickwrapId, clickwrapVersion, clickwrapRequest);
        }

        private static ClickwrapRequest BuildUpdateClickwrapVersionRequest()
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Status = "active"
            };

            return clickwrapRequest;
        }
    }
}
