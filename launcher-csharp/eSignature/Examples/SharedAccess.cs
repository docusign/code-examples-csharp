// <copyright file="SharedAccess.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>
namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using OAuth = DocuSign.eSign.Client.Auth.OAuth;

    public static class SharedAccess
    {
        public static OAuth.UserInfo GetCurrentUserInfo(string basePath, string accessToken)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            return docuSignClient.GetUserInfo(accessToken);
        }

        public static UserInformation GetUserInfo(
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
            //ds-snippet-start:eSign43Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var usersApi = new UsersApi(docuSignClient);
            //ds-snippet-end:eSign43Step2

            //ds-snippet-start:eSign43Step3
            var newUser = new NewUsersDefinition
            {
                NewUsers = new List<UserInformation>
                {
                    new UserInformation(activationCode, Email: agentEmail, UserName: agentName),
                },
            };

            var userSummary = usersApi.Create(accountId, newUser);
            //ds-snippet-end:eSign43Step3

            return userSummary;
        }

        public static EnvelopesInformation GetEnvelopesListStatus(
            string accessToken,
            string basePath,
            string accountId,
            string userId)
        {
            //ds-snippet-start:eSign43Step5
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            docuSignClient.Configuration.DefaultHeader.Add("X-DocuSign-Act-On-Behalf", userId);

            var envelopesApi = new EnvelopesApi(docuSignClient);

            var date = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mmZ");
            var option = new EnvelopesApi.ListStatusChangesOptions()
            {
                fromDate = date,
            };

            var envelopes = envelopesApi.ListStatusChanges(accountId, option);
            //ds-snippet-end:eSign43Step5

            return envelopes;
        }

        public static void CreateUserAuthorization(
            string accessToken,
            string basePath,
            string accountId,
            string userId,
            string agentUserId)
        {
            //ds-snippet-start:eSign43Step4
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

            //ds-snippet-end:eSign43Step4
        }
    }
}