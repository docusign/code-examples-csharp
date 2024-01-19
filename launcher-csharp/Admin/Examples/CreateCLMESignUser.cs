// <copyright file="CreateCLMESignUser.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class CreateClmeSignUser
    {
        /// <summary>
        /// Creates a new user to be used in both CLM and eSignature products
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
        /// <returns>The AddUserResponse object coming back from the API</returns>
        public static AddUserResponse Create(string userName, string firstName, string lastName, string email, string clmPermissionProfileId, string eSignPermissionProfileId, Guid? dsGroupId, Guid? clmProductId, Guid? eSignProductId, string basePath, string accessToken, Guid? accountId, Guid? orgId)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            UsersApi usersApi = new UsersApi(apiClient);

            //ds-snippet-start:Admin2Step5
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
            newMultiProductUserAddRequest.AutoActivateMemberships = true;
            //ds-snippet-end:Admin2Step5

            //ds-snippet-start:Admin2Step6
            AddUserResponse response = usersApi.AddOrUpdateUser(orgId, accountId, newMultiProductUserAddRequest);
            //ds-snippet-end:Admin2Step6

            return response;
        }
    }
}
