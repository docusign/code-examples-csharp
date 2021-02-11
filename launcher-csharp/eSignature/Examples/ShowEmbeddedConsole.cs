using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class ShowEmbeddedConsole
    {
        /// <summary>
        /// Generates a URL to be use to embedded a view of an envelope in your application
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="startingView">The sending view to show initially (either "tagging" or "recipient")</param>
        /// <param name="returnUrl">Url user will be redirected to after they sign</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>URL for the embedded console for this envelope</returns>
        public static string CreateEmbeddedConsoleView(string accessToken, string basePath,
            string accountId, string startingView, string returnUrl, string envelopeId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            ConsoleViewRequest viewRequest = MakeConsoleViewRequest(returnUrl,
                startingView, envelopeId);

            // Step 1. create the NDSE view
            // Call the CreateSenderView API
            // Exceptions will be caught by the calling function
            ViewUrl results = envelopesApi.CreateConsoleView(accountId, viewRequest);
            string redirectUrl = results.Url;
            return redirectUrl;
        }

        private static ConsoleViewRequest MakeConsoleViewRequest(string dsReturnUrl, string startingView,
            string envelopeId)
        {
            // Data for this method
            // dsReturnUrl
            // startingView
            // envelopeId

            ConsoleViewRequest viewRequest = new ConsoleViewRequest();
            // Set the URL where you want the recipient to go once they are done
            // with the NDSE. It is usually the case that the
            // user will never "finish" with the NDSE.
            // Assume that control will not be passed back to your app.
            viewRequest.ReturnUrl = dsReturnUrl;

            if ("envelope".Equals(startingView) && envelopeId != null)
            {
                viewRequest.EnvelopeId = envelopeId;
            }

            return viewRequest;
        }
    }
}
