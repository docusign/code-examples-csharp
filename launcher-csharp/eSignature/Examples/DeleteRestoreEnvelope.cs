// <copyright file="DeleteRestoreEnvelope.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public class DeleteRestoreEnvelope
    {
        private const string DeleteFolderId = "recyclebin";

        /// <summary>
        /// Delete envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">Envelope ID</param>
        /// <returns>The folders response</returns>
        public static FoldersResponse DeleteEnvelope(
            string accessToken,
            string basePath,
            string accountId,
            string envelopeId)
        {
            //ds-snippet-start:eSign45Step2
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign45Step2

            FoldersApi foldersApi = new FoldersApi(docusignClient);
            //ds-snippet-start:eSign45Step3
            var foldersRequest = new FoldersRequest
            {
                EnvelopeIds = new List<string> { envelopeId },
            };
            //ds-snippet-end:eSign45Step3
            //ds-snippet-start:eSign45Step4
            var response = foldersApi.MoveEnvelopesWithHttpInfo(accountId, DeleteFolderId, foldersRequest);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:eSign45Step4
        }

        /// <summary>
        /// Moves envelope to a different folder
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">Envelope ID</param>
        /// <param name="folderId">Folder ID</param>
        /// <param name="fromFolderId">From folder ID</param>
        /// <returns>The folders response</returns>
        public static FoldersResponse MoveEnvelopeToFolder(
            string accessToken,
            string basePath,
            string accountId,
            string envelopeId,
            string folderId,
            string fromFolderId = null)
        {
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            FoldersApi foldersApi = new FoldersApi(docusignClient);
            var foldersRequest = new FoldersRequest
            {
                FromFolderId = fromFolderId,
                EnvelopeIds = new List<string> { envelopeId },
            };

            //ds-snippet-start:eSign45Step6
            var response = foldersApi.MoveEnvelopesWithHttpInfo(accountId, folderId, foldersRequest);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:eSign45Step6
        }

        /// <summary>
        /// Gets the list of folders
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The folders response</returns>
        public static FoldersResponse GetFolders(
            string accessToken,
            string basePath,
            string accountId)
        {
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            FoldersApi foldersApi = new FoldersApi(docusignClient);
            var response = foldersApi.ListWithHttpInfo(accountId);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
        }

        /// <summary>
        /// Gets folder by name
        /// </summary>
        /// <param name="folders">List of folders to search</param>
        /// <param name="targetName">Name of the folder to find</param>
        /// <returns>The folder</returns>
        public static Folder GetFolderByName(List<Folder> folders, string targetName)
        {
            foreach (Folder folder in folders)
            {
                if (folder.Name.Equals(targetName))
                {
                    return folder;
                }

                //ds-snippet-start:eSign45Step5
                if (folder.Folders != null && !folder.Folders.Count.Equals(0))
                {
                    Folder nestedFolder = GetFolderByName(folder.Folders, targetName);
                    if (nestedFolder != null)
                    {
                        return nestedFolder;
                    }
                }

                //ds-snippet-end:eSign45Step5
            }

            return null;
        }
    }
}
