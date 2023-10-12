// <copyright file="DeleteUserProductPermissionProfileById.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class DeleteUserProductPermissionProfileById
    {
        /// <summary>
        /// Delete user product permission profiles using an email address.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign account Id (GUID)</param>
        /// <param name="emailAddress">The email address of the user (GUID)</param>
        /// <param name="productId">The DocuSign product Id (GUID)</param>
        /// <returns>Result of the remove product action</returns>
        public static RemoveUserProductsResponse DeleteUserProductPermissionProfile(
            string basePath,
            string accessToken,
            Guid? orgId,
            Guid accountId,
            string emailAddress,
            Guid? productId)
        {
            //ds-snippet-start:Admin9Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin9Step2

            var productPermissionProfilesApi = new ProductPermissionProfilesApi(apiClient);
            //ds-snippet-start:Admin9Step4
            var userProductProfileDeleteRequest = new UserProductProfileDeleteRequest
            {
                ProductIds = new List<Guid?>
                {
                    productId,
                },
                UserEmail = emailAddress,
            };
            //ds-snippet-end:Admin9Step4

            //ds-snippet-start:Admin9Step5
            return productPermissionProfilesApi.RemoveUserProductPermission(orgId, accountId, userProductProfileDeleteRequest);
            //ds-snippet-end:Admin9Step5
        }

        /// <summary>
        /// Get permission profiles by email.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign account Id (GUID)</param>
        /// <param name="emailAddress">The email address of DocuSign user (string)</param>
        /// <returns>Product permission profiles</returns>
        public static UserProductPermissionProfilesResponse GetPermissionProfilesByEmail(
            string basePath,
            string accessToken,
            Guid? orgId,
            Guid accountId,
            string emailAddress)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            //ds-snippet-start:Admin9Step3
            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);

            var getUserProductPermission = new ProductPermissionProfilesApi.GetUserProductPermissionProfilesByEmailOptions
            {
                email = emailAddress,
            };

            return productPermissionProfileApi.GetUserProductPermissionProfilesByEmail(orgId, accountId, getUserProductPermission);
            //ds-snippet-end:Admin9Step3
        }
    }
}
