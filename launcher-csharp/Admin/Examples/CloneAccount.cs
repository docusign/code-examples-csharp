// <copyright file="CloneAccount.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class CloneAccount
    {
        private static readonly string AuthorizationHeader = "Authorization";
        private static readonly string Bearer = "Bearer ";

        /// <summary>
        /// Get all accounts in asset groups for the organization. Required scopes: asset_group_account_read
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <returns>AssetGroupAccountsResponse</returns>
        public static AssetGroupAccountsResponse GetGroupAccounts(string basePath, string accessToken, Guid? orgId)
        {
            //ds-snippet-start:Admin12Step3
            DocuSignClient docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add(AuthorizationHeader, Bearer + accessToken);

            ProvisionAssetGroupApi assetGroupApi = new ProvisionAssetGroupApi(docuSignClient);
            ProvisionAssetGroupApi.GetAssetGroupAccountsOptions options = new ProvisionAssetGroupApi.GetAssetGroupAccountsOptions
            {
                compliant = true,
            };

            return assetGroupApi.GetAssetGroupAccounts(orgId, options);
            //ds-snippet-end:Admin12Step3
        }

        /// <summary>
        /// Clones an existing DocuSign account to a new DocuSign account.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="sourceAccountId">Source account Id (GUID)</param>
        /// <param name="targetAccountName">Target account name (string)</param>
        /// <param name="targetAccountFirstName">Target account first name (string)</param>
        /// <param name="targetAccountLastName">Target account last name (string)</param>
        /// <param name="targetAccountEmail">Target account email (string)</param>
        /// <returns>AssetGroupAccountClone</returns>
        public static AssetGroupAccountClone CloneGroupAccount(
            string basePath,
            string accessToken,
            Guid? orgId,
            Guid? sourceAccountId,
            string targetAccountName,
            string targetAccountFirstName,
            string targetAccountLastName,
            string targetAccountEmail)
        {
            //ds-snippet-start:Admin12Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add(AuthorizationHeader, Bearer + accessToken);
            //ds-snippet-end:Admin12Step2

            //ds-snippet-start:Admin12Step4
            string countryCode = "US";
            var accountData = new AssetGroupAccountClone
            {
                SourceAccount = new AssetGroupAccountCloneSourceAccount
                {
                    Id = sourceAccountId,
                },
                TargetAccount = new AssetGroupAccountCloneTargetAccount
                {
                    Name = targetAccountName,
                    Admin = new AssetGroupAccountCloneTargetAccountAdmin
                    {
                        FirstName = targetAccountFirstName,
                        LastName = targetAccountLastName,
                        Email = targetAccountEmail,
                    },
                    CountryCode = countryCode,
                },
            };
            //ds-snippet-end:Admin12Step4

            //ds-snippet-start:Admin12Step5
            var assetGroupApi = new ProvisionAssetGroupApi(docuSignClient);
            return assetGroupApi.CloneAssetGroupAccount(orgId, accountData);
            //ds-snippet-end:Admin12Step5
        }
    }
}
