// <copyright file="WebQueryEndpointFunc.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Monitor.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Monitor.Api;
    using DocuSign.Monitor.Client;
    using DocuSign.Monitor.Model;

    public class WebQueryEndpointFunc
    {
        /// <summary>
        /// Gets companies data from monitor
        /// </summary>
        /// <param name="requestPath">Request path, used for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (JWT OAuth)</param>
        /// <param name="accountId">Account id</param>
        /// <param name="filterStartDate">Start date for the filtering</param>
        /// <param name="filterEndDate">End date for the filtering</param>
        /// <returns>The json, containing companies data from monitor</returns>
        public virtual IEnumerable<object> Invoke(
            string accessToken,
            string requestPath,
            string accountId,
            string filterStartDate,
            string filterEndDate)
        {
            try
            {
                ApiClient apiClient = new ApiClient(ApiClient.Demo_REST_BasePath);

                // Construct API headers
                // step 2 start
                apiClient.SetBasePath(ApiClient.Demo_REST_BasePath);
                apiClient.Configuration.DefaultHeader.Add("Authorization", string.Format("Bearer {0}", accessToken));
                apiClient.Configuration.DefaultHeader.Add("Content-Type", "application/json");

                // step 2 end

                // Declare variables
                // step 4 start
                DataSetApi dataSetApi = new DataSetApi(apiClient);
                WebQuery options = this.PrepareWebQuery(filterStartDate, filterEndDate, accountId);

                var cursoredResult = dataSetApi.PostWebQuery("2.0", "monitor", options);

                // step 4 end
                return cursoredResult.Result;
            }
            catch (ApiException)
            {
                return new string[] { "ERROR", "You do not have Monitor enabled for your account, follow <a target='_blank' href='https://developers.docusign.com/docs/monitor-api/how-to/enable-monitor/'>How to enable Monitor for your account</a> to get it enabled." };
            }
        }

        // Step 3 start
        private WebQuery PrepareWebQuery(string filterStartDate, string filterEndDate, string accountId)
        {
            return new WebQuery
            {
                Filters = new List<object>
                {
                    new
                    {
                        FilterName = "Time",
                        BeginTime = filterStartDate,
                        EndTime = filterEndDate,
                    },
                    new
                    {
                        FilterName = "Has",
                        ColumnName = "AccountId",
                        Value = accountId,
                    },
                },
                Aggregations = new List<object>
                {
                    new
                    {
                        aggregationName = "Raw",
                        limit = "100",
                        orderby = new string[] { "Timestamp, desc" },
                    },
                },
            };
        }

        // Step 3 end
    }
}
