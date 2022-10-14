using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeneralCargoSystem.Areas.GCAnalytics.Controllers
{
    [Area("GCAnalytics")]
    [Authorize]
    public class DashboardController : Controller
    {

        public DashboardController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
