using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class CreateEnvelopeFromTemplate
    {
        /// <summary>
        /// Creates a new envelope from an existing template and send it out
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="templateId">The templateId for the tempalte to use to create an envelope</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public static string SendEnvelopeFromTemplate(string signerEmail, string signerName, string ccEmail, string ccName, string accessToken, string basePath, string accountId, string templateId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, templateId);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(accountId, envelope);
            return result.EnvelopeId;
        }

        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName,
            string ccEmail, string ccName, string templateId)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // templateId

            EnvelopeDefinition env = new EnvelopeDefinition();
            env.TemplateId = templateId;

            TemplateRole signer1 = new TemplateRole();
            signer1.Email = signerEmail;
            signer1.Name = signerName;
            signer1.RoleName = "signer";

            TemplateRole cc1 = new TemplateRole();
            cc1.Email = ccEmail;
            cc1.Name = ccName;
            cc1.RoleName = "cc";

            env.TemplateRoles = new List<TemplateRole> { signer1, cc1 };
            env.Status = "sent";
            return env;
        }
    }
}
