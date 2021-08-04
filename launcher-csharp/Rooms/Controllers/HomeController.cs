using System.Diagnostics;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    public class HomeController : Controller
    {
        private IRequestItemsService _requestItemsService { get; }

        public HomeController(IRequestItemsService requestItemsService)
        {
            _requestItemsService = requestItemsService;
        }

        public IActionResult Index(string egName)
        {
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
    }
}
