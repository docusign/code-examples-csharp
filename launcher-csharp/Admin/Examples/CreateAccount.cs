// <copyright file="CreateAccount.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class CreateAccount
    {
        private const string AuthorizationHeader = "Authorization";
        private const string BearerPrefix = "Bearer ";
        private const string DefaultAccountName = "CreatedThroughAPI";
        private const string DefaultCountryCode = "US";

        /// <summary>
        /// Get all plan items and return the first. Required scopes: organization_sub_account_read
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <returns>First available OrganizationSubscriptionResponse</returns>
        public static OrganizationSubscriptionResponse GetFirstPlanItem(
            string basePath,
            string accessToken,
            Guid? orgId)
        {
            //ds-snippet-start:Admin13Step2
            var docuSignClient = GetConfiguredClient(basePath, accessToken);
            var assetGroupApi = new ProvisionAssetGroupApi(docuSignClient);
            //ds-snippet-end:Admin13Step2

            //ds-snippet-start:Admin13Step3
            return assetGroupApi.GetOrganizationPlanItems(orgId).FirstOrDefault();
            //ds-snippet-end:Admin13Step3
        }

        /// <summary>
        /// Create an account by subscription id and plan id. Required scopes: organization_sub_account_write
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="email">User email</param>
        /// <param name="firstName">User first name</param>
        /// <param name="lastName">User last name</param>
        /// <param name="subscriptionId">Subscription Id (string)</param>
        /// <param name="planId">Plan Id (GUID)</param>
        /// <returns>Result of account creation</returns>
        public static SubscriptionProvisionModelAssetGroupWorkResult CreateAccountBySubscription(
            string basePath,
            string accessToken,
            Guid? orgId,
            string email,
            string firstName,
            string lastName,
            string subscriptionId,
            Guid? planId)
        {
            var docuSignClient = GetConfiguredClient(basePath, accessToken);
            var assetGroupApi = new ProvisionAssetGroupApi(docuSignClient);

            //ds-snippet-start:Admin13Step4
            var subAccountRequest = BuildSubAccountRequest(email, firstName, lastName, subscriptionId, planId);
            //ds-snippet-end:Admin13Step4

            //ds-snippet-start:Admin13Step5
            return assetGroupApi.CreateAssetGroupAccount(orgId, subAccountRequest);
            //ds-snippet-end:Admin13Step5
        }

        /// <summary>
        /// Helper method to configure DocuSign client with authorization headers.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <returns>Configured DocuSignClient</returns>
        private static DocuSignClient GetConfiguredClient(string basePath, string accessToken)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader[AuthorizationHeader] = BearerPrefix + accessToken;
            return docuSignClient;
        }

        /// <summary>
        /// Helper method to build SubAccountCreateRequest object.
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="firstName">User first name</param>
        /// <param name="lastName">User last name</param>
        /// <param name="subscriptionId">Subscription Id (string)</param>
        /// <param name="planId">Plan Id (GUID)</param>
        /// <returns>Configured SubAccountCreateRequest object</returns>
        //ds-snippet-start:Admin13Step4
        private static SubAccountCreateRequest BuildSubAccountRequest(
            string email,
            string firstName,
            string lastName,
            string subscriptionId,
            Guid? planId)
        {
            return new SubAccountCreateRequest
            {
                SubscriptionDetails = new SubAccountCreateRequestSubAccountCreationSubscription
                {
                    Id = subscriptionId,
                    PlanId = planId,
                    Modules = new List<Guid?>(),
                },
                TargetAccount = new SubAccountCreateRequestSubAccountCreationTargetAccountDetails
                {
                    Name = DefaultAccountName,
                    CountryCode = DefaultCountryCode,
                    Admin = new SubAccountCreateRequestSubAccountCreationTargetAccountAdmin
                    {
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        Locale = SubAccountCreateRequestSubAccountCreationTargetAccountAdmin.LocaleEnum.En,
                    },
                },
            };
        }
    }

    //ds-snippet-end:Admin13Step4
}
