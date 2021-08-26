using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocuSign.CodeExamples.Models;
using Microsoft.Extensions.Configuration;

namespace DocuSign.CodeExamples.Controllers
{
    public class HomeController : Controller
    {
        private IRequestItemsService _requestItemsService { get; }
        private IConfiguration _configuration { get;  }

        public HomeController(IRequestItemsService requestItemsService, IConfiguration configuration)
        {
            _requestItemsService = requestItemsService;
            _configuration = configuration;
        }

        public IActionResult Index(string egName)
        {
            if (_configuration["quickstart"] == "true")
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    _configuration["quickstart"] = "false";
                }
                return Redirect("eg001");
            }
            if (egName == "home")
            {
                return View();
            }
            if (string.IsNullOrEmpty(egName))
            {
                egName = _requestItemsService.EgName;
            }
            if (!string.IsNullOrWhiteSpace(egName))
            {
                _requestItemsService.EgName = null;
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
