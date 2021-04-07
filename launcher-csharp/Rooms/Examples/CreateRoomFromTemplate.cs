using System.Collections.Generic;
using System.Linq;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;

namespace DocuSign.Rooms.Examples
{
    public class CreateRoomFromTemplate
    {
        /// <summary>
        /// Gets the list of room temlates
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The list of room templates</returns>
        public static RoomTemplatesSummaryList GetTemplates(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomTemplatesApi = new RoomTemplatesApi(apiClient);

            // Call the Rooms API to create a room
            return roomTemplatesApi.GetRoomTemplates(accountId);
        }

        /// <summary>
        /// Creates a room using specified template
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="model">The model of room</param>
        /// <param name="templateId">The Id of room template</param>
        /// <returns>The instance of created room</returns>
        public static Room CreateRoom(
            string basePath,
            string accessToken,
            string accountId,
            RoomModel model,
            int templateId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var roomsApi = new RoomsApi(apiClient);
            var rolesApi = new RolesApi(apiClient);

            // Obtain Role
            var clientRole = rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Default Admin" }).Roles.First();

            // Construct the request body for your room
            var newRoom = BuildRoom(model, clientRole, templateId);

            // Call the Rooms API to create a room
            return roomsApi.CreateRoom(accountId, newRoom);
        }

        private static RoomForCreate BuildRoom(
            RoomModel model,
            RoleSummary clientRole,
            int? templateId)
        {
            var newRoom = new RoomForCreate
            {
                Name = model.Name,
                RoleId = clientRole.RoleId,
                TemplateId = templateId,
                FieldData = new FieldDataForCreate
                {
                    Data = new Dictionary<string, object>
                    {
                        { "address1", "Street 1" },
                        { "address2", "Unit 10" },
                        { "city", "New York" },
                        { "postalCode", "11112" },
                        { "companyRoomStatus", "5" },
                        { "state", "US-NY" },
                        { "comments", @"New room for sale." }
                    }
                }
            };

            return newRoom;
        }

        public class RoomModel
        {
            public string Name { get; set; }
            public int TemplateId { get; set; }
            public IEnumerable<RoomTemplate> Templates { get; set; }
        }
    }
}
