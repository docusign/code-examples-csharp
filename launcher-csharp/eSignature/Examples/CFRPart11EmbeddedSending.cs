namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class CFRPart11EmbeddedSending
    {

        /// <summary>
        /// Checks if account is CFR Part 11 enabled
        /// </summary>
        /// <param name="accessToken">Access Token to make API calls</param>
        /// <param name="basePath">BasePath to make API calls</param>
        /// <param name="accountId">AccountId (GUID) for this account</param>
        /// <returns>True if CFR Part 11, false otherwise</returns>
        public static bool IsCFRPart11Account(string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Call the eSignature REST API
            AccountsApi accountsApi = new AccountsApi(apiClient);

            var accountSettingsInformation = accountsApi.ListSettings(accountId);

            return (accountSettingsInformation.Require21CFRpt11Compliance == "true");
        }
    }
}
