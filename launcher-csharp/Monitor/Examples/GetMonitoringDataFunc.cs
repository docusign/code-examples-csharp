// <copyright file="GetMonitoringDataFunc.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Monitor.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Monitor.Api;
    using DocuSign.Monitor.Client;
    using static DocuSign.Monitor.Api.DataSetApi;

    public class GetMonitoringDataFunc
    {
        /// <summary>
        /// Gets data from monitor
        /// </summary>
        /// <param name="requestPath">Request path, used for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (JWT OAuth)</param>
        /// <returns>The list of JObjects, containing data from monitor</returns>
        public virtual IEnumerable<object> Invoke(string accessToken, string requestPath)
        {
            try
            {
                DocuSignClient apiClient = new DocuSignClient(DocuSignClient.Demo_REST_BasePath);

                // Construct API headers
                //ds-snippet-start:Monitor1Step2
                apiClient.SetBasePath(DocuSignClient.Demo_REST_BasePath);
                apiClient.Configuration.DefaultHeader.Add("Authorization", string.Format("Bearer {0}", accessToken));
                apiClient.Configuration.DefaultHeader.Add("Content-Type", "application/json");
                //ds-snippet-end:Monitor1Step2

                // Declare variables
                //ds-snippet-start:Monitor1Step3
                bool complete = false;
                string cursorValue = string.Empty;
                int limit = 2; // Amount of records you want to read in one request
                List<object> functionResult = new List<object>();

                DataSetApi dataSetApi = new DataSetApi(apiClient);
                GetStreamOptions options = new GetStreamOptions();

                options.limit = limit;

                // Get monitoring data
                do
                {
                    if (!string.IsNullOrEmpty(cursorValue))
                    {
                        options.cursor = cursorValue;
                    }

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

                //ds-snippet-end:Monitor1Step3
                return functionResult;
            }
            catch (ApiException)
            {
                return new string[] { "ERROR", "You do not have Monitor enabled for your account, follow <a target='_blank' href='https://developers.docusign.com/docs/monitor-api/how-to/enable-monitor/'>How to enable Monitor for your account</a> to get it enabled." };
            }
        }
    }
}
