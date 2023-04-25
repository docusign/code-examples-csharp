// <copyright file="SharedAccess.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace ESignature.Examples
{
    public static class SharedAccess
    {
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
                    new UserInformation(activationCode, Email: agentEmail, UserName: agentName)
                }
            };

            return usersApi.Create(accountId, newUser);

            // Step 2 end
        }
    }
}