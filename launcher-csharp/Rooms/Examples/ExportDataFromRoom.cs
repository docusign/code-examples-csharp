// <copyright file="ExportDataFromRoom.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using System;
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class ExportDataFromRoom
    {
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

            // Call the Rooms API to create a room
            var response = roomsApi.GetRoomsWithHttpInfo(accountId);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
        }

        /// <summary>
        /// Gets the specified room's field data
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="roomId">The Id of a specified room</param>
        /// <returns>The specified room's field data</returns>
        public static FieldData Export(
            string basePath,
            string accessToken,
            string accountId,
            int roomId)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms3Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            //ds-snippet-end:Rooms3Step2

            // Call the Rooms API to get room field data
            //ds-snippet-start:Rooms3Step3
            var response = roomsApi.GetRoomFieldDataWithHttpInfo(accountId, roomId);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
            //ds-snippet-end:Rooms3Step3
        }
    }
}
