using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocuSign.CodeExamples.Models;

namespace DocuSign.CodeExamples.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return Redirect("eg001");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/dsReturn")]
        public IActionResult DsReturn()
        {            
            return Redirect("eg001");
        }
    }
}
