// <copyright file="AddingFormToRoom.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class AddingFormToRoom
    {
        /// <summary>
        /// Gets the list of rooms and forms
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The tuple with lists of rooms and forms</returns>
        public static (FormSummaryList forms, RoomSummaryList rooms) GetFormsAndRooms(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms4Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            var formLibrariesApi = new FormLibrariesApi(apiClient);
            //ds-snippet-end:Rooms4Step2

            // Get Forms Libraries
            //ds-snippet-start:Rooms4Step3
            ApiResponse<FormLibrarySummaryList> formLibraries = formLibrariesApi.GetFormLibrariesWithHttpInfo(accountId);

            formLibraries.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            formLibraries.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            // Get Forms
            ApiResponse<FormSummaryList> forms = null;
            if (formLibraries.Data.FormsLibrarySummaries.Any())
            {
                forms = formLibrariesApi.GetFormLibraryFormsWithHttpInfo(
                    accountId,
                    formLibraries.Data.FormsLibrarySummaries.First().FormsLibraryId);
            }

            forms.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            forms.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            //ds-snippet-end:Rooms4Step3

            // Get Rooms
            ApiResponse<RoomSummaryList> rooms = roomsApi.GetRoomsWithHttpInfo(accountId);

            rooms.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            rooms.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            // Call the Rooms API to create a room
            return (forms.Data, rooms.Data);
        }

        /// <summary>
        /// Adds form to specified room
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="roomId">The Id of a specified room</param>
        /// <param name="formId">The Id of a specified form</param>
        /// <returns>RoomDocument</returns>
        public static RoomDocument AddForm(
            string basePath,
            string accessToken,
            string accountId,
            int roomId,
            Guid formId)
        {
            // Construct your API headers
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);

            // Call the Rooms API to get room field data
            //ds-snippet-start:Rooms4Step4
            var response = roomsApi.AddFormToRoomWithHttpInfo(accountId, roomId, new FormForAdd(formId));
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
            //ds-snippet-end:Rooms4Step4
        }
    }
}
