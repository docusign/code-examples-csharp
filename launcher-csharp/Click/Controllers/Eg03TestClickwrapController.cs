using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
    [Route("[area]/Eg03")]
    public class Eg03TestClickwrapController : EgController
    {
        public Eg03TestClickwrapController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg03";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            if (string.IsNullOrEmpty(RequestItemsService.ClickwrapId))
            {
                ViewBag.errorCode = 400;
                ViewBag.errorMessage = "Cannot find any clickwrap. Please first create a clickwrap using the first example.";

                return View("Error");
            }

            return base.Get();
        }

        protected override void InitializeInternal()
        {            
            ViewBag.ClickwrapId = RequestItemsService.ClickwrapId;
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }       
    }
}