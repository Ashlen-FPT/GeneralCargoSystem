using GeneralCargoSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralCargoSystem.Areas.GCAnalytics.Controllers
{
    [Area("GCAnalytics")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context= context;
        }

        public IActionResult Index()
        {
            var bookingsToday = _context.GCBookings.Where(x=>x.Date==DateTime.Today).Include(g => g.Commodity).Include(g => g.FPTSites).Include(g => g.LogisticalTransporter).ToList();

            ViewBag.Bookings = bookingsToday;
            return View();
        }
    }
}
