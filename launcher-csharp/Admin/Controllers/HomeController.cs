using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
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
                return Redirect($"Admin/{egName}");
            }

            return View();
        }
    }
}