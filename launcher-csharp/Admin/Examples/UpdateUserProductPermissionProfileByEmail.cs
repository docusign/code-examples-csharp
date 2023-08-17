// <copyright file="UpdateUserProductPermissionProfileByEmail.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class UpdateUserProductPermissionProfileByEmail
    {
        /// <summary>
        /// Update user product permission profile by email address.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign account Id (GUID)</param>
        /// <param name="emailAddress">The email of the user (string)</param>
        /// <param name="productId">The DocuSign Product Id (GUID)</param>
        /// <param name="permissionProfileId">The DocuSign Permission profile Id (GUID)</param>
        /// <returns>Result of the user update method</returns>
        public static UserProductPermissionProfilesResponse UpdateUserProductPermissionProfile(
            string basePath,
            string accessToken,
            Guid? orgId,
            Guid accountId,
            string emailAddress,
            Guid? productId,
            string permissionProfileId)
        {
            //ds-snippet-start:Admin8Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin8Step2
            var productPermissionProfilesApi = new ProductPermissionProfilesApi(apiClient);

            //ds-snippet-start:Admin8Step3
            var userProductPermissionProfilesRequest = new UserProductPermissionProfilesRequest
            {
                Email = emailAddress,
                ProductPermissionProfiles = new List<ProductPermissionProfileRequest>
                {
                    new ProductPermissionProfileRequest
                    {
                        PermissionProfileId = permissionProfileId,
                        ProductId = productId,
                    },
                },
            };
            //ds-snippet-end:Admin8Step3

            //ds-snippet-start:Admin8Step4
            return productPermissionProfilesApi.AddUserProductPermissionProfilesByEmail(
                orgId,
                accountId,
                userProductPermissionProfilesRequest);
            //ds-snippet-end:Admin8Step4
        }

        /// <summary>
        /// Get permission profiles.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign account Id (GUID)</param>
        /// <returns>Product permission profiles</returns>
        public static ProductPermissionProfilesResponse GetPermissionProfiles(
            string basePath,
            string accessToken,
            Guid? orgId,
            Guid accountId)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);
            return productPermissionProfileApi.GetProductPermissionProfiles(orgId, accountId);
        }
    }
}
