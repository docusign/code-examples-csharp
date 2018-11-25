using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg004")]
    public class Eg004EnvelopeInfoController : EgController
    {
        public Eg004EnvelopeInfoController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Get envelope information";
        }

        public override string EgName => "eg004";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            EnvelopesApi envelopesApi = new EnvelopesApi(RequestItemsService.DefaultConfiguration);
            ViewBag.h1 = "Get envelope status results";
            ViewBag.message  = "Results from the Envelopes::get method:";
            DocuSign.eSign.Model.Envelope results = envelopesApi.GetEnvelope(RequestItemsService.Session.AccountId, RequestItemsService.EnvelopeId);
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            
            return View("example_done");
        }
    }
}