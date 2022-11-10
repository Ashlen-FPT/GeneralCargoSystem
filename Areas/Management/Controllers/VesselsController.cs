using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GeneralCargoSystem.Data;
using GeneralCargoSystem.Models.GC;
using Microsoft.AspNetCore.Authorization;
using GeneralCargoSystem.Utility;
using System.Configuration;
using GeneralCargoSystem.Models;
using System.Security.Claims;

namespace GeneralCargoSystem.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = Enums.Supervisor)]
    [NoDirectAccess]
    public class VesselsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VesselsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Management/Vessels
        public async Task<IActionResult> Index()
        {
            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
            var log = new Logs
            {
                UserEmail = email,
                UserName = findUsername,
                LogType = Enums.Read,
                AffectedTable = "Vessels",
                AdditionalData = "All Vessels Read",
                DateTime = DateTime.Now
            };
            _context.Logs.Add(log);
            _context.SaveChanges();

            return View(await _context.Vessels.ToListAsync());
        }

        // GET: Management/Vessels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vessels == null)
            {
                return NotFound();
            }

            var vessels = await _context.Vessels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vessels == null)
            {
                return NotFound();
            }

            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
            var log = new Logs
            {
                UserEmail = email,
                UserName = findUsername,
                LogType = Enums.Read,
                AffectedTable = "Vessels",
                AdditionalData = "Expanded Vessel Details",
                DateTime = DateTime.Now
            };
            _context.Logs.Add(log);
            _context.SaveChanges();

            return PartialView(vessels);
            //return View(vessels);
        }

        // GET: Management/Vessels/Create
        public IActionResult Create()
        {
            return PartialView();
            //return View();
        }

        // POST: Management/Vessels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,VesselName")]*/ Vessels vessels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vessels);

                var email = User.Identity!.Name!;
                var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
                var log = new Logs
                {
                    UserEmail = email,
                    UserName = findUsername,
                    LogType = Enums.Created,
                    AffectedTable = "Vessels",
                    DateTime = DateTime.Now
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            return View(vessels);
        }

        // GET: Management/Vessels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vessels == null)
            {
                return NotFound();
            }

            var vessels = await _context.Vessels.FindAsync(id);
            if (vessels == null)
            {
                return NotFound();
            }
            return PartialView(vessels);
            //return View(vessels);
        }

        // POST: Management/Vessels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("Id,VesselName")]*/ Vessels vessels)
        {
            if (id != vessels.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vessels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VesselsExists(vessels.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var email = User.Identity!.Name!;
                var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
                var log = new Logs
                {
                    UserEmail = email,
                    UserName = findUsername,
                    LogType = Enums.Updated,
                    AffectedTable = "Vessels",
                    DateTime = DateTime.Now
                };
                _context.Logs.Add(log);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }


            return View(vessels);
        }

        // GET: Management/Vessels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vessels == null)
            {
                return NotFound();
            }

            var vessels = await _context.Vessels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vessels == null)
            {
                return NotFound();
            }

            return PartialView(vessels);
            //return View(vessels);
        }

        // POST: Management/Vessels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vessels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vessels'  is null.");
            }
            var vessels = await _context.Vessels.FindAsync(id);
            if (vessels != null)
            {
                _context.Vessels.Remove(vessels);
            }
            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;
            var log = new Logs
            {
                UserEmail = email,
                UserName = findUsername,
                LogType = Enums.Deleted,
                AffectedTable = "Vessels",
                DateTime = DateTime.Now
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VesselsExists(int id)
        {
            return _context.Vessels.Any(e => e.Id == id);
        }
    }
}
