// <copyright file="SharedAccess.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
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
                var informationList = usersApi.ListWithHttpInfo(accountId, callListOptions);
                informationList.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
                informationList.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                if (reset != null && remaining != null)
                {
                    Console.WriteLine("API calls remaining: " + remaining);
                    Console.WriteLine("Next Reset: " + resetDate);
                }

                if (int.Parse(informationList.Data.ResultSetSize) > 0)
                {
                    userInformation = informationList.Data.Users.First(user => user.UserStatus == "Active");
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

            var userSummary = usersApi.CreateWithHttpInfo(accountId, newUser);
            userSummary.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            userSummary.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign43Step3

            return userSummary.Data;
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

            var envelopes = envelopesApi.ListStatusChangesWithHttpInfo(accountId, option);
            envelopes.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            envelopes.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:eSign43Step5

            return envelopes.Data;
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
            var userAuthorizations = accountApi.GetAgentUserAuthorizationsWithHttpInfo(accountId, agentUserId, options);
            userAuthorizations.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            userAuthorizations.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            if (userAuthorizations.Data.Authorizations == null || userAuthorizations.Data.Authorizations.Count == 0)
            {
                var authRequest = new UserAuthorizationCreateRequest(
                    Permission: managePermission,
                    AgentUser: new AuthorizationUser(AccountId: accountId, UserId: agentUserId));

                var createUserAuthorization = accountApi.CreateUserAuthorizationWithHttpInfo(accountId, userId, authRequest);
                createUserAuthorization.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
                createUserAuthorization.Headers.TryGetValue("X-RateLimit-Reset", out reset);

                if (reset != null && remaining != null)
                {
                    DateTime resetDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                    Console.WriteLine("API calls remaining: " + remaining);
                    Console.WriteLine("Next Reset: " + resetDateTime);
                }
            }

            //ds-snippet-end:eSign43Step4
        }
    }
}