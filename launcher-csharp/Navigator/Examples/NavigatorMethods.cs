// <copyright file="NavigatorMethods.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class NavigatorMethods
    {
        /// <summary>
        /// Lists all agreements for the specified account using the IAM client.
        /// </summary>
        /// <returns>AgreementsResponse</returns>
        public static async Task<AgreementsResponse> ListAgreementsWithIamClient(
            string basePath, string accessToken, string accountId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Navigator.Agreements.GetAgreementsListAsync(accountId);
        }

        /// <summary>
        /// Retrieves a specific agreement by its ID using the IAM client.
        /// </summary>
        /// <returns>Agreement</returns>
        public static async Task<Agreement> GetAgreementWithIamClient(
            string basePath, string accessToken, string accountId, string agreementId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Navigator.Agreements.GetAgreementAsync(accountId, agreementId);
        }

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
