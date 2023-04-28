using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralCargoSystem.Data;
using System.Collections.Generic;
using GeneralCargoSystem.Models;
using GeneralCargoSystem.Utility;
using GeneralCargoSystem.Models.GC;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GeneralCargoSystem.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = Enums.Supervisor)]
    [NoDirectAccess]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IEnumerable<SelectListItem> RoleList = new List<SelectListItem>();

        public UserController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
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
                user.Role = roles.FirstOrDefault(u => u.Id == roleId)!.Name;
            }

            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
            var log = new Logs
            {
                UserEmail = email,
                UserName = findUsername,
                LogType = Enums.Read,
                AffectedTable = "Users",
                DateTime = DateTime.Now
            };
            _context.Logs.Add(log);
            _context.SaveChanges();

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
            if (data.UserStatus == Enums.ActiveUser)
            {
                data.UserStatus = Enums.InactiveUser;
            }
            else
            if (data.UserStatus == Enums.InactiveUser)
            {
                data.UserStatus = Enums.ActiveUser;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Successful." });
        }

        public IActionResult EditRole()
        {
            RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {

                Text = i,
                Value = i

            });

            ViewData["Roles"] = RoleList;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, ApplicationUser user)
        {

            var findRole = _context.UserRoles.Find(id);
            if (id != user.Id)
            {
                return NotFound();
            }

            if (user != null)
            {
                try
                {

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;
                    //if (!RoleExists(user.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                TempData["Update"] = "User Role Updated";
                return RedirectToAction(nameof(Index));
            }
            RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {

                Text = i,
                Value = i

            });

            ViewData["Roles"] = RoleList;
            return View(user);
        }

        private bool RoleExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
