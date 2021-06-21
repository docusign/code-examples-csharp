using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg05")]
    public class Eg05GetClickwrapResponsesController : EgController
    {
        public Eg05GetClickwrapResponsesController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg05";
        protected override void InitializeInternal()
        {
            ViewBag.ClickwrapId = RequestItemsService.ClickwrapId;
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }    
    }
}