using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using static DocuSign.eSign.Api.EnvelopesApi;

namespace eSignature.Examples
{
    public class UnpauseSignatureWorkflow
    {
        /// <summary>
        /// Unpauses signature workflow
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="pausedEnvelopeId">The Envelope ID which workflow will be unpaused</param>
        /// <returns>The update summary of the envelopes</returns>
        public static EnvelopeUpdateSummary UnpauseWorkflow(string accessToken, string basePath, string accountId, string pausedEnvelopeId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Construct request body
            var envelope = MakeEnvelope();

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            var updateOptions = new UpdateOptions() { resendEnvelope = "true" };
            return envelopesApi.Update(accountId, pausedEnvelopeId, envelope, updateOptions);
        }

        private static Envelope MakeEnvelope()
        {
            return new Envelope
            {
                Workflow = new Workflow()
                {
                    WorkflowStatus = "in_progress"
                }
            };
        }
    }
}
