using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace DocuSign.CodeExamples.Admin.Examples
{
    public class ImportUser
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="csvFileData">The user data in csv format</param>
        /// <returns>The response of creating a new user</returns>
        public static JObject CreateBulkImportRequest(string accessToken, string basePath, Guid? organizationId, string csvFileData)
        {
            var requestPath = $"{basePath}/v2/organizations/{organizationId}/imports/bulk_users/add";

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
            headers.Add("Content-Disposition", "filename=userData.csv");
            headers.Add("Content-Type", "text/csv");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestPath);
            request.Headers = headers;
            request.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] userData = encoding.GetBytes(csvFileData);
            request.ContentLength = userData.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(userData);            

            Stream responseStream = request.GetResponse().GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream);

            string result = responseStreamReader.ReadToEnd();

            JObject resultJson = JObject.Parse(result);

            return resultJson;
        }
    }
}
