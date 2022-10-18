using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralCargoSystem.Data;
using System.Collections.Generic;
using GeneralCargoSystem.Models;

namespace GeneralCargoSystem.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize]
    public class SystemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Users()
        {
            //var userList = new List<ApplicationUser>();
            //userList = _context.ApplicationUsers.ToList();
            var userList = _context.ApplicationUsers.ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;

            }

            return Json(new { data = userList });
        }
    }
}
