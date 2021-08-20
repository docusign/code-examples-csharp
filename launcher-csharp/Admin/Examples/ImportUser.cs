using System;
using System.IO;
using System.Text;
using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;

namespace DocuSign.CodeExamples.Admin.Examples
{
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var bulkImportsApi = new BulkImportsApi(apiClient);

            var csvFileData = File.ReadAllText(csvFilePath)
                .Replace("{account_id}", accountId);

            var bytes = Encoding.UTF8.GetBytes(csvFileData);

            return bulkImportsApi.CreateBulkImportAddUsersRequest(organizationId, bytes);
        }
    }
}
