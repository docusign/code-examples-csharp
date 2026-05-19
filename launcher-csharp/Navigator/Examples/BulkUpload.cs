// <copyright file="BulkUpload.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;
    using BulkJob = Docusign.IAM.SDK.Models.Components.BulkJob;

    public class BulkUpload
    {
        //ds-snippet-start:Navigator3Step2

        public static async Task<BulkJob> CreateBulkUpload(string basePath, string accessToken, string accountId, string jobName)
        {
            var client = IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();

            var bulkUploadJob = new CreateBulkJob
            {
                JobName = jobName,
                ExpectedNumberOfDocs = 5,
                Language = "en-US",
            };

            return await client.AgreementManager.BulkJob.CreateBulkUploadJobAsync(bulkUploadJob, accountId);
        }

        //ds-snippet-end:Navigator3Step2

        //ds-snippet-start:Navigator3Step3
        public static async Task UploadDocuments(string[] uploadUrls, string[] filePaths)
        {
            var httpClient = new HttpClient();
            var count = Math.Min(uploadUrls.Length, filePaths.Length);

            for (int i = 0; i < count; ++i)
            {
                string uploadUrl = uploadUrls[i];
                string filePath = filePaths[i];

                string fileName = Path.GetFileName(filePath);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Skipping {fileName} - file not found: {filePath}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(uploadUrl))
                {
                    Console.WriteLine($"Skipping {fileName} - no upload URL found");
                    continue;
                }

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                string contentType = GetContentType(fileName);

                var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
                request.Headers.TryAddWithoutValidation("x-ms-blob-type", "BlockBlob");
                request.Headers.TryAddWithoutValidation("x-ms-meta-filename", fileName);
                request.Content = new StreamContent(fileStream);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(
                        $"Failed to upload {fileName}. " +
                        $"Status: {(int)response.StatusCode} {response.ReasonPhrase}. " +
                        $"Response: {error}");

                    continue;
                }
            }
        }

        //ds-snippet-end:Navigator3Step3

        //ds-snippet-start:Navigator3Step4
        public static async Task<BulkJob> CompleteBulkUploadJob(string basePath, string accessToken, string accountId, string jobId)
        {
            var client = IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();

            return await client.AgreementManager.BulkJob.UploadCompleteBulkJobAsync(accountId, jobId);
        }

        //ds-snippet-end:Navigator3Step4

        private static string GetContentType(string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();

            return extension switch
            {
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".pdf" => "application/pdf",
                ".html" => "text/html",
                ".txt" => "text/plain",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }
    }
}