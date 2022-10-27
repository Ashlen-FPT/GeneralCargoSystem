using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralCargoSystem.Data;
using System.Collections.Generic;
using GeneralCargoSystem.Models;
using GeneralCargoSystem.Utility;

namespace GeneralCargoSystem.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize]
    public class LogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            var logs = _context.Logs.ToList();

            return Json(new { data = logs });
        }
    }
}
