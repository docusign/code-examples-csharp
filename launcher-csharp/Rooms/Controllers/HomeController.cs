using System.Diagnostics;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
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
    }
}
