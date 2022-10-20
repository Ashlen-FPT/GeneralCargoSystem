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
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
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

        [HttpPost]
        public IActionResult UserStatus([FromBody] string id)
        {
            var data = _context.ApplicationUsers.Find(id);
            if (data == null)
            {
                return Json(new { success = false, message = "Error while Activating/Deactivating" });
            }
            if (data.UserStatus==Enums.ActiveUser)
            {
                data.UserStatus = Enums.InactiveUser;
            }
            else
            if(data.UserStatus==Enums.InactiveUser)
            {
                data.UserStatus = Enums.ActiveUser;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Successful." });
        }
    }
}
