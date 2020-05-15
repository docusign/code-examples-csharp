using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Authorization;
using eg_03_csharp_auth_code_grant_core.Common;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    public class HomeController : Controller
    {
        public IRequestItemsService RequestItemsService { get; }

        public HomeController(IRequestItemsService requestItemsService)
        {
            RequestItemsService = requestItemsService;
        }

        public IActionResult Index()
        {
            string egName = RequestItemsService.EgName;
            if (!string.IsNullOrWhiteSpace(egName))
            {
                RequestItemsService.EgName = null;
                return Redirect(egName);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/dsReturn")]
        public IActionResult DsReturn(string state, string @event, string envelopeId)
        {            
            ViewBag.title = "Return from DocuSign";
            ViewBag._event = @event;
            ViewBag.state = state;
            ViewBag.envelopeId = envelopeId;

            return View();
        }
    }
}
