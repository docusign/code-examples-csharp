// <copyright file="SharedAccess.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using DocuSign.Admin.Client.Auth;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.Extensions.Logging;
using OAuth = DocuSign.eSign.Client.Auth.OAuth;

namespace ESignature.Examples
{
    public static class SharedAccess
    {

        public static OAuth.UserInfo getCurrentUserInfo(string basePath, string accessToken)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            return docuSignClient.GetUserInfo(accessToken);
        }

        public static UserInformation getUserInfo(
            string accessToken,
            string basePath,
            string accountId,
            string agentEmail)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var usersApi = new UsersApi(docuSignClient);

            UserInformation userInformation = null;
            var callListOptions = new UsersApi.ListOptions
            {
                email = agentEmail,
            };

            try
            {
                var informationList = usersApi.List(accountId, callListOptions);

                if (int.Parse(informationList.ResultSetSize) > 0)
                {
                    userInformation = informationList.Users.First(user => user.UserStatus == "Active");
                }
            }
            catch (ApiException apiException)
            {
                Console.WriteLine(apiException.ErrorMessage);
            }

            return userInformation;
        }

        public static NewUsersSummary ShareAccess(
            string accessToken,
            string basePath,
            string accountId,
            string activationCode,
            string agentEmail,
            string agentName)
        {
            // Step 1 start
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var usersApi = new UsersApi(docuSignClient);

            // Step 1 end

            // Step 2 start
            var newUser = new NewUsersDefinition
            {
                NewUsers = new List<UserInformation>
                {
                    new UserInformation(activationCode, Email: agentEmail, UserName: agentName),
                },
            };

            var userSummary = usersApi.Create(accountId, newUser);

            // Step 2 end

            return userSummary;
        }

        public static EnvelopesInformation GetEnvelopesListStatus(
            string accessToken,
            string basePath,
            string accountId,
            string userId)
        {
            // Step 1 start
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            docuSignClient.Configuration.DefaultHeader.Add("X-DocuSign-Act-On-Behalf", userId);
            
            var envelopesApi = new EnvelopesApi(docuSignClient);

            // Step 1 end

            // Step 2 start
            var date = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mmZ");
            var option = new EnvelopesApi.ListStatusChangesOptions()
            {
                fromDate = date
            };

            var envelopes = envelopesApi.ListStatusChanges(accountId, option);

            // Step 2 end
            return envelopes;
        }


        public static void CreateUserAuthorization(
            string accessToken,
            string basePath,
            string accountId,
            string userId,
            string agentUserId)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var accountApi = new AccountsApi(docuSignClient);

            var managePermission = "manage";
            var options = new AccountsApi.GetAgentUserAuthorizationsOptions
            {
                permissions = managePermission,
            };
            UserAuthorizations userAuthorizations = accountApi.GetAgentUserAuthorizations(accountId, agentUserId, options);

            if (userAuthorizations.Authorizations == null || userAuthorizations.Authorizations.Count == 0)
            {
                var authRequest = new UserAuthorizationCreateRequest(
                    Permission: managePermission,
                    AgentUser: new AuthorizationUser(AccountId: accountId, UserId: agentUserId));

                accountApi.CreateUserAuthorization(accountId, userId, authRequest);
            }
        }
    }
}