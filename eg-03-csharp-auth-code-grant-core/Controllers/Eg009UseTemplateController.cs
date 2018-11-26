using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg009")]
    public class Eg009UseTemplateController : EgController
    {
        public Eg009UseTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg009";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            var config = new Configuration(new ApiClient(session.BasePath + "/restapi"));
            config.AddDefaultHeader("Authorization", "Bearer " + user.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, RequestItemsService.TemplateId);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(RequestItemsService.Session.AccountId, envelope);            
            RequestItemsService.EnvelopeId = result.EnvelopeId;
            ViewBag.message = "The envelope has been created and sent!<br/>Envelope ID " + result.EnvelopeId + ".";
            return View("example_done");
        }

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, 
            string ccEmail, string ccName, string templateId)
        {
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
    }
}