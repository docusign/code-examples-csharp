using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg009")]
    public class Eg009UseTemplateController : EgController
    {
        public Eg009UseTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg009";

        // ***DS.snippet.0.start
        string DoWork (string signerEmail, string signerName, string ccEmail,
            string ccName, string accessToken, string basePath,
            string accountId, string templateId)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // accessToken
            // basePath
            // accountId
            // templateId

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, templateId);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(accountId, envelope);
            return result.EnvelopeId;
        }

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, 
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
            signer1.Name =  signerName;
            signer1.RoleName = "signer";

            TemplateRole cc1 = new TemplateRole();
            cc1.Email = ccEmail;
            cc1.Name = ccName;
            cc1.RoleName = "cc";

            env.TemplateRoles = new List<TemplateRole> { signer1, cc1 };
            env.Status = "sent";
            return env;
        }
        // ***DS.snippet.0.end

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var templateId = RequestItemsService.TemplateId;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            string envelopeId = DoWork(signerEmail, signerName, ccEmail,
                ccName, accessToken, basePath, accountId, templateId);

            RequestItemsService.EnvelopeId = envelopeId;
            ViewBag.message = "The envelope has been created and sent!<br/>Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}