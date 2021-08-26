using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
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
                return Redirect($"Click/{egName}");
            }

            return View();
        }
    }
}