using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Model;

namespace DocuSign.Click.Examples
{
    public class GetClickwrapResponses
    {
        /// <summary>
        /// Gets a clickwrap agreements
        /// </summary>
        /// <param name="clickwrapId">The Id of clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The agreements of a given clickwrap</returns>
        public static ClickwrapAgreementsResponse GetAgreements(string clickwrapId, string basePath, string accessToken, string accountId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            return clickAccountApi.GetClickwrapAgreements(accountId, clickwrapId);
        }
    }
}
