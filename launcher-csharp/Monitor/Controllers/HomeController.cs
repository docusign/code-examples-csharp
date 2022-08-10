using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Monitor.Controllers
{
    [Area("Monitor")]
    public class HomeController : Controller
    {
        private IRequestItemsService _requestItemsService { get; }
        private LauncherTexts _launcherTexts { get; }

        public HomeController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts)
        {
            _requestItemsService = requestItemsService;
            _launcherTexts = launcherTexts;
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
                return Redirect($"/{egName}");
            }

            ViewBag.APITexts = _launcherTexts.ManifestStructure.Groups;
            return View();
        }
    }
}