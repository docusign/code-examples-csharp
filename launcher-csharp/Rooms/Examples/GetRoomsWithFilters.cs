using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;

namespace DocuSign.Rooms.Examples
{
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            
            // Call the Rooms API to get room field data
            var rooms = roomsApi.GetRooms(accountId, new RoomsApi.GetRoomsOptions
            {
                fieldDataChangedStartDate = fieldDataChangedStartDate,
                fieldDataChangedEndDate = fieldDataChangedEndDate
            });
            
            return rooms;
        }
    }
}
