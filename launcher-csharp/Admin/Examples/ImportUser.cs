// <copyright file="ImportUser.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Examples
{
    using System;
    using System.IO;
    using System.Text;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class ImportUser
    {
        /// <summary>
        /// Adds users via bulk import
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="csvFilePath">Path to the user data csv file</param>
        /// <returns>The response of users import</returns>
        public static OrganizationImportResponse CreateBulkImportRequest(string accessToken, string basePath, string accountId, Guid? organizationId, string csvFilePath)
        {
            //ds-snippet-start:Admin4Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin4Step2

            //ds-snippet-start:Admin4Step3
            var bulkImportsApi = new BulkImportsApi(apiClient);

            var csvFileData = File.ReadAllText(csvFilePath)
                .Replace("{account_id}", accountId);

            var bytes = Encoding.UTF8.GetBytes(csvFileData);

            var response = bulkImportsApi.CreateBulkImportAddUsersRequestWithHttpInfo(organizationId, bytes);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            return response.Data;
            //ds-snippet-end:Admin4Step3
        }

        /// <summary>
        /// Checks the status of an existing bulk import requests
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="importId">Unique ID of the bulk user import request</param>
        /// <returns>OrganizationImportResponse</returns>
        public static OrganizationImportResponse CheckkStatus(string accessToken, string basePath, Guid? organizationId, Guid? importId)
        {
            //ds-snippet-start:Admin4Step4
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var bulkImportsApi = new BulkImportsApi(apiClient);

            var response = bulkImportsApi.GetBulkUserImportRequestWithHttpInfo(organizationId, importId);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            return response.Data;
            //ds-snippet-end:Admin4Step4
        }
    }
}
