using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace DocuSign.CodeExamples.Admin.Examples
{
    public class BulkExportUserData
    {
        private static BulkExportsApi _bulkExportsApi = null;

        /// <summary>
        /// Gets the user data by using bulk-export
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="filePath">Path to a file where the user data will be saved</param>
        /// <returns></returns>
        public static OrganizationExportResponse CreateBulkExportRequest(string accessToken, string basePath,
            Guid? organizationId, string filePath)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            _bulkExportsApi = new BulkExportsApi(apiClient);

            var organizationExportRequest = new OrganizationExportRequest
            {
                Type = "organization_memberships_export"
            };

            var exportResponse = _bulkExportsApi.CreateUserListExport(organizationId, organizationExportRequest);
            int retryCount = 5;

            while (retryCount >= 0)
            {
                if(exportResponse.Status == OrganizationExportResponse.StatusEnum.Completed)
                {
                    GetUserData(accessToken, organizationId, exportResponse.Id, filePath);
                    break;
                }
                else
                {
                    --retryCount;
                    System.Threading.Thread.Sleep(5000);
                    exportResponse = _bulkExportsApi.GetUserListExport(organizationId, exportResponse.Id);
                }
            }

            return exportResponse;
        }

        /// <summary>
        /// Downloads exported user data
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="exportId">The DocuSign bulk-export ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="filePath">Path to a file where the user data will be saved</param>
        /// <returns></returns>
        private static void GetUserData(string accessToken, Guid? organizationId, Guid? exportId, string filePath)
        {
            var bulkExportResponse = _bulkExportsApi.GetUserListExport(organizationId, exportId);
            var csvUrl = bulkExportResponse.Results.FirstOrDefault().Url;

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
            headers.Add("Content-Type", "application/json");

            HttpWebRequest request;
            HttpWebResponse response = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(csvUrl);
                request.Headers = headers;
                request.Timeout = 10000;
                request.AllowWriteStreamBuffering = false;

                response = (HttpWebResponse)request.GetResponse();

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
            }
            catch (WebException)
            {
                if (response != null)
                    response.Close();
            }
        }
    }
}
