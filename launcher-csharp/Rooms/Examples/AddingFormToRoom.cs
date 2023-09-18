// <copyright file="AddingFormToRoom.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
            FormLibrarySummaryList formLibraries = formLibrariesApi.GetFormLibraries(accountId);

            // Get Forms
            FormSummaryList forms = new FormSummaryList(new List<FormSummary>());
            if (formLibraries.FormsLibrarySummaries.Any())
            {
                forms = formLibrariesApi.GetFormLibraryForms(
                    accountId,
                    formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
            }

            //ds-snippet-end:Rooms4Step3

            // Get Rooms
            RoomSummaryList rooms = roomsApi.GetRooms(accountId);

            // Call the Rooms API to create a room
            return (forms, rooms);
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
            return roomsApi.AddFormToRoom(accountId, roomId, new FormForAdd(formId));
            //ds-snippet-end:Rooms4Step4
        }
    }
}
