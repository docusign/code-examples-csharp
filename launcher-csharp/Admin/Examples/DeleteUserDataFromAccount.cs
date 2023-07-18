// <copyright file="DeleteUserDataFromAccount.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public static class DeleteUserDataFromAccount
    {
        /// <summary>
        /// Delete user data from account.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign account Id (GUID)</param>
        /// <param name="userId">The DocuSign user Id (GUID)</param>
        /// <returns>Result of the delete user action</returns>
        public static IndividualUserDataRedactionResponse DeleteUserDataFromAccountByUserId(
            string basePath,
            string accessToken,
            Guid accountId,
            string userId)
        {
            //ds-snippet-start:Admin11Step2
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var accountsApi = new AccountsApi(docusignClient);
            //ds-snippet-end:Admin11Step2

            //ds-snippet-start:Admin11Step3
            var membershipDataRedaction = new IndividualMembershipDataRedactionRequest(Guid.Parse(userId));
            //ds-snippet-end:Admin11Step3

            //ds-snippet-start:Admin11Step4
            return accountsApi.RedactIndividualMembershipData(accountId, membershipDataRedaction);
            //ds-snippet-end:Admin11Step4
        }
    }
}
