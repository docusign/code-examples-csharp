using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
            //  Construct API headers
            // step 2 start
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
            headers.Add("Content-Type", "application/json");
            // step 2 end

            // Declare variables
            // step 3 start
            bool complete = false;
            string cursorValue = "";
            int limit = 2; // Amount of records you want to read in one request
            List<JObject> functionResult = new List<JObject>();

            // Get monitoring data
            do
            {
                var cursorValueFormatted = (!string.IsNullOrEmpty(cursorValue)) ? $"={cursorValue}" : cursorValue;

                // Add cursor value and amount of records to read to the request
                var requestParameters = String.Format("stream?cursor{0}&limit={1}",
                cursorValueFormatted, limit);

                WebRequest request = WebRequest.Create(requestPath + requestParameters);
                request.Headers = headers;

                Stream requestStream = request.GetResponse().GetResponseStream();
                StreamReader requestStreamReader = new StreamReader(requestStream);

                string result = requestStreamReader.ReadToEnd();

                // Parse result to JSON format
                JObject resultJson = JObject.Parse(result);
                string endCursor = resultJson.GetValue("endCursor").ToString();

                // If the endCursor from the response is the same as the one that you already have,
                // it means that you have reached the end of the records
                if (endCursor == cursorValue)
                {
                    complete = true;
                }
                else
                {
                    cursorValue = endCursor;
                    functionResult.Add(resultJson);
                }
            } 
            while (!complete);
            // step 3 end

            return functionResult;
        }
    }
}

