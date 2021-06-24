using DocuSign.OrgAdmin.Api;
using DocuSign.OrgAdmin.Client;
using DocuSign.OrgAdmin.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.Admin.Examples
{
    public class CreateCLMESignUser
    {
        /// <summary>
        /// Creates a new clickwrap
        /// </summary>
        /// <param name="userName">New user's user name</param>
        /// <param name="firstName">New user's first name</param>
        /// <param name="lastName">New user's last name</param>
        /// <param name="email">New user's email address</param>
        /// <param name="clmPermissionProfileId">CLM Permission Profile Id (GUID)</param>
        /// <param name="eSignPermissionProfileId">eSignature Permission Profile Id (GUID)</param>
        /// <param name="dsGroupId">DocuSign Group Id (GUID)</param>
        /// <param name="clmProductId">CLM Product Id (GUID)</param>
        /// <param name="eSignProductId">eSignature Product Id (GUID)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign Account Id (GUID)</param>
        public static Guid? Create(string userName, string firstName, string lastName, string email, string clmPermissionProfileId, string eSignPermissionProfileId, Guid? dsGroupId, Guid? clmProductId, Guid? eSignProductId, string basePath, string accessToken, Guid? accountId, Guid? orgId)
        {
            var apiClient = new ApiClient("https://api-d.docusign.net/management");
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            UsersApi usersApi = new UsersApi(apiClient);
            var newMultiProductUserAddRequest = new NewMultiProductUserAddRequest();
            newMultiProductUserAddRequest.UserName = userName;
            newMultiProductUserAddRequest.FirstName = firstName;
            newMultiProductUserAddRequest.LastName = lastName;
            newMultiProductUserAddRequest.Email = email;
            var productPermissionProfiles = new List<ProductPermissionProfileRequest>();
            productPermissionProfiles.Add(new ProductPermissionProfileRequest { ProductId = clmProductId, PermissionProfileId = clmPermissionProfileId });
            productPermissionProfiles.Add(new ProductPermissionProfileRequest { ProductId = eSignProductId, PermissionProfileId = eSignPermissionProfileId });
            newMultiProductUserAddRequest.ProductPermissionProfiles = productPermissionProfiles;
            newMultiProductUserAddRequest.DsGroups = new List<DSGroupRequest>();
            newMultiProductUserAddRequest.DsGroups.Add(new DSGroupRequest { DsGroupId = dsGroupId });
            AddUserResponse response = usersApi.AddUsers_0(orgId, accountId, newMultiProductUserAddRequest);
            return response.Id;
        }

    }
}
