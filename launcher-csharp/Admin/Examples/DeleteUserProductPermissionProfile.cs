using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.Admin.Examples
{
    public class DeleteUserProductPermissionProfileById
    {
        /// <summary>
        /// Delete user product permission profiles using an email address
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var productPermissionProfilesApi = new ProductPermissionProfilesApi(apiClient);
            
            var userProductProfileDeleteRequest = new UserProductProfileDeleteRequest
            {
                ProductIds = new List<Guid?>
                {
                    productId
                },
                UserEmail = emailAddress
            };

            return productPermissionProfilesApi.RemoveUserProductPermission(orgId, accountId, userProductProfileDeleteRequest);
        }

        /// <summary>
        /// Get permission profiles by email
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);

            var getUserProductPermission = new ProductPermissionProfilesApi.GetUserProductPermissionProfilesByEmailOptions
            {
                email = emailAddress
            };

            return productPermissionProfileApi.GetUserProductPermissionProfilesByEmail(orgId, accountId, getUserProductPermission);
        }
    }
}
