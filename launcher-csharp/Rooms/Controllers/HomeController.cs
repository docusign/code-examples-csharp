using System.Diagnostics;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Controllers
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
