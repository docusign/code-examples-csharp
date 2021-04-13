using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Monitor.Controllers
{
    [Area("Monitor")]
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
                return Redirect($"Monitor/{egName}");
            }

            return View();
        }
    }
}