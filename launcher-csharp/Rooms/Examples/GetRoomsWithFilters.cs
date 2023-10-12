// <copyright file="GetRoomsWithFilters.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class GetRoomsWithFilters
    {
        /// <summary>
        /// Gets the list of rooms by filter
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="fieldDataChangedStartDate">The start date</param>
        /// <param name="fieldDataChangedEndDate">The end date</param>
        /// <returns>The filtered room summary list</returns>
        public static RoomSummaryList GetRooms(
            string basePath,
            string accessToken,
            string accountId,
            string fieldDataChangedStartDate,
            string fieldDataChangedEndDate)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms5Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            //ds-snippet-end:Rooms5Step2

            // Call the Rooms API to get room field data
            //ds-snippet-start:Rooms5Step4
            var rooms = roomsApi.GetRooms(accountId, new RoomsApi.GetRoomsOptions
            {
                fieldDataChangedStartDate = fieldDataChangedStartDate,
                fieldDataChangedEndDate = fieldDataChangedEndDate,
            });
            //ds-snippet-end:Rooms5Step4

            return rooms;
        }
    }
}
