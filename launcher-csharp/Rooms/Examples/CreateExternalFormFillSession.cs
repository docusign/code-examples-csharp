﻿// <copyright file="CreateExternalFormFillSession.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class CreateExternalFormFillSession
    {
        /// <summary>
        /// Gets the list of Room Documents
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// /// <param name="roomId">The Id of a specified room</param>
        /// <returns>The list of Room Documents</returns>
        public static RoomDocumentList GetDocuments(
            string basePath,
            string accessToken,
            string accountId,
            int? roomId)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms6Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            //ds-snippet-end:Rooms6Step2

            // Call the Rooms API to get Room Documents
            return roomsApi.GetDocuments(accountId, roomId);
        }

        /// <summary>
        /// Gets the list of rooms
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The list of rooms</returns>
        public static RoomSummaryList GetRooms(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);

            // Call the Rooms API to get the list of rooms
            return roomsApi.GetRooms(accountId);
        }

        /// <summary>
        /// Creates external form fill session
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="documentId">The Id of a specified document</param>
        /// <param name="roomId">The Id of a specified room</param>
        /// <returns>ExternalFormFillSession</returns>
        //ds-snippet-start:Rooms6Step3
        public static ExternalFormFillSession CreateSession(
            string basePath,
            string accessToken,
            string accountId,
            ExternalFormFillSessionForCreate sessionForCreate)
        //ds-snippet-end:Rooms6Step3
        {
            // Construct your API headers
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var externalFormFillSessionsApi = new ExternalFormFillSessionsApi(apiClient);

            // Call the Rooms API to create external form fill session
            //ds-snippet-start:Rooms6Step4
            var url = externalFormFillSessionsApi.CreateExternalFormFillSession(accountId, sessionForCreate);

            return url;
            //ds-snippet-end:Rooms6Step4
        }
    }
}
