// <copyright file="BulkExportUserData.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Examples
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class BulkExportUserData
    {
        /// <summary>
        /// Gets the user data by using bulk-export
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="filePath">Path to a file where the user data will be saved</param>
        /// <returns>OrganizationExportResponse</returns>
        public static OrganizationExportResponse CreateBulkExportRequest(
            string accessToken,
            string basePath,
            Guid? organizationId,
            string filePath)
        {
            //ds-snippet-start:Admin3Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin3Step2

            //ds-snippet-start:Admin3Step3
            var bulkExportsApi = new BulkExportsApi(apiClient);

            var organizationExportRequest = new OrganizationExportRequest
            {
                Type = "organization_memberships_export",
            };

            var exportResponse = bulkExportsApi.CreateUserListExportWithHttpInfo(organizationId, organizationExportRequest);
            exportResponse.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            exportResponse.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            //ds-snippet-end:Admin3Step3

            //ds-snippet-start:Admin3Step4
            int retryCount = 5;

            while (retryCount >= 0)
            {
                if (exportResponse.Data.Results != null)
                {
                    GetUserData(accessToken, exportResponse.Data.Results.FirstOrDefault().Url, filePath);
                    break;
                }
                else
                {
                    --retryCount;
                    System.Threading.Thread.Sleep(5000);
                    exportResponse = bulkExportsApi.GetUserListExportWithHttpInfo(organizationId, exportResponse.Data.Id);
                    exportResponse.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
                    exportResponse.Headers.TryGetValue("X-RateLimit-Reset", out reset);

                    resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

                    Console.WriteLine("API calls remaining: " + remaining);
                    Console.WriteLine("Next Reset: " + resetDate);
                }
            }

            //ds-snippet-end:Admin3Step4
            return exportResponse.Data;
        }

        /// <summary>
        /// Downloads exported user data
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="csvUrl">URL to get the csv file with exported user data</param>
        /// <param name="filePath">Path to a file where the user data will be saved</param>
        private static void GetUserData(string accessToken, string csvUrl, string filePath)
        {
            WebHeaderCollection headers = new WebHeaderCollection
            {
                { "Authorization", string.Format("Bearer {0}", accessToken) },
                { "Content-Type", "application/json" },
            };

            HttpWebRequest request;
            HttpWebResponse response = null;

            try
            {
                //ds-snippet-start:Admin3Step5
                request = (HttpWebRequest)WebRequest.Create(csvUrl);
                request.Headers = headers;
                request.Timeout = 10000;
                request.AllowWriteStreamBuffering = false;

                response = (HttpWebResponse)request.GetResponse();
                string remaining = response.Headers.Get("X-RateLimit-Remaining");
                string reset = response.Headers.Get("X-RateLimit-Reset");

                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);

                Stream stream = response.GetResponseStream();

                FileStream fileStream = new FileStream(filePath, FileMode.Create);

                byte[] buffer = new byte[256];
                int count = stream.Read(buffer, 0, buffer.Length);

                while (count > 0)
                {
                    fileStream.Write(buffer, 0, count);
                    count = stream.Read(buffer, 0, buffer.Length);
                }

                fileStream.Close();
                stream.Close();
                response.Close();
                //ds-snippet-end:Admin3Step5
            }
            catch (WebException)
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
