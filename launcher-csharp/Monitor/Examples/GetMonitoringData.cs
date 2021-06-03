using System;
using System.Collections.Generic;
using DocuSign.Monitor.Api;
using DocuSign.Monitor.Client;
using static DocuSign.Monitor.Api.DataSetApi;

namespace DocuSign.CodeExamples.Monitor.Examples
{
    public class GetMonitoringDataFunc
    {
        /// <summary>
        /// Gets data from monitor
        /// </summary>
        /// <param name="requestPath">Request path, used for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (JWT OAuth)</param>
        /// <returns>The list of JObjects, containing data from monitor</returns>
        public virtual IEnumerable<Object> Invoke(string accessToken, string requestPath)
        {
            ApiClient apiClient = new ApiClient(ApiClient.Demo_REST_BasePath);
            
            //  Construct API headers
            // step 2 start
            apiClient.SetBasePath(ApiClient.Demo_REST_BasePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", String.Format("Bearer {0}", accessToken));
            apiClient.Configuration.DefaultHeader.Add("Content-Type", "application/json");
            // step 2 end

            // Declare variables
            // step 3 start
            bool complete = false;
            string cursorValue = "";
            int limit = 2; // Amount of records you want to read in one request
            List<object> functionResult = new List<object>();
            
            DataSetApi dataSetApi = new DataSetApi(apiClient);
            GetStreamOptions options = new GetStreamOptions();

            options.limit = limit;

            // Get monitoring data
            do
            {
                if (!string.IsNullOrEmpty(cursorValue))
                    options.cursor = cursorValue;

                var cursoredResult = dataSetApi.GetStreamWithHttpInfo("2.0", "monitor", options);

                string endCursor = cursoredResult.Data.EndCursor;

                // If the endCursor from the response is the same as the one that you already have,
                // it means that you have reached the end of the records
                if (endCursor == cursorValue)
                {
                    complete = true;
                }
                else
                {
                    cursorValue = endCursor;
                    functionResult.Add(cursoredResult.Data);
                }
            } 
            while (!complete);
            // step 3 end

            return functionResult;
        }
    }
}

