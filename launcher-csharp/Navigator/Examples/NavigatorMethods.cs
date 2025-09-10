// <copyright file="NavigatorMethods.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;
    using Docusign.IAM.SDK.Models.Requests;

    public static class NavigatorMethods
    {
        /// <summary>
        /// Lists all agreements for the specified account using the IAM client.
        /// </summary>
        /// <returns>AgreementsResponse</returns>
        //ds-snippet-start:Navigator1Step3
        public static async Task<AgreementsResponse> ListAgreementsWithIamClient(
            string basePath, string accessToken, string accountId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Navigator.Agreements.GetAgreementsListAsync(new GetAgreementsListRequest
            {
                AccountId = accountId,
            });
        }

        //ds-snippet-end:Navigator1Step3

        /// <summary>
        /// Retrieves a specific agreement by its ID using the IAM client.
        /// </summary>
        /// <returns>Agreement</returns>
        //ds-snippet-start:Navigator2Step3
        public static async Task<Agreement> GetAgreementWithIamClient(
            string basePath, string accessToken, string accountId, string agreementId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Navigator.Agreements.GetAgreementAsync(accountId, agreementId);
        }

        //ds-snippet-end:Navigator2Step3

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        //ds-snippet-start:NavigatorCsharpStep2
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }

        //ds-snippet-end:NavigatorCsharpStep2
    }
}
