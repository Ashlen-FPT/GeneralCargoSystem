using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GeneralCargoSystem.Data;
using GeneralCargoSystem.Models.GC;
using GeneralCargoSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace GeneralCargoSystem.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = Enums.Supervisor)]
    [NoDirectAccess]
    public class LogisticalTransportersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogisticalTransportersController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region RemoteValidations

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsTransporterExist(string name, int Id = 0)
        {

            bool isExist = _context.LogisticalTransporters.Any(x => x.Name == name && x.Id != Id);
            if (isExist == true)
            {
                return Json(data: false);
            }
            else
            {
                return Json(data: true);
            }
        }

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsRegistrationNoExist(string registrationNo, int Id = 0)
        {

            bool isExist = _context.LogisticalTransporters.Any(x => x.RegistrationNo == registrationNo && x.Id != Id);
            if (isExist == true)
            {
                return Json(data: false);
            }
            else
            {
                return Json(data: true);
            }
        }
        #endregion

        // GET: Management/LogisticalTransporters
        public async Task<IActionResult> Index()
        {

            ViewBag.Created = TempData["Create"] as string;
            ViewBag.Updated = TempData["Update"] as string;
            ViewBag.Deleted = TempData["Delete"] as string;

            return View(await _context.LogisticalTransporters.ToListAsync());
        }

        // GET: Management/LogisticalTransporters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LogisticalTransporters == null)
            {
                return NotFound();
            }

            var logisticalTransporter = await _context.LogisticalTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logisticalTransporter == null)
            {
                return NotFound();
            }

            return PartialView(logisticalTransporter);
            //return View(logisticalTransporter);
        }

        // GET: Management/LogisticalTransporters/Create
        public IActionResult Create()
        {
            return PartialView();
            //return View();
        }

        // POST: Management/LogisticalTransporters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,Name,RegistrationNo")]*/ LogisticalTransporter logisticalTransporter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(logisticalTransporter);
                await _context.SaveChangesAsync();
                TempData["Create"] = "Logistical Transporter Created";
                return RedirectToAction(nameof(Index));
            }
            return View(logisticalTransporter);
        }

        // GET: Management/LogisticalTransporters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LogisticalTransporters == null)
            {
                return NotFound();
            }

            var logisticalTransporter = await _context.LogisticalTransporters.FindAsync(id);
            if (logisticalTransporter == null)
            {
                return NotFound();
            }
            return PartialView(logisticalTransporter);
            //return View(logisticalTransporter);
        }

        // POST: Management/LogisticalTransporters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("Id,Name,RegistrationNo")]*/ LogisticalTransporter logisticalTransporter)
        {
            if (id != logisticalTransporter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logisticalTransporter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogisticalTransporterExists(logisticalTransporter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Update"] = "Logistical Transporter Updated";
                return RedirectToAction(nameof(Index));
            }
            return View(logisticalTransporter);
        }

        // GET: Management/LogisticalTransporters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LogisticalTransporters == null)
            {
                return NotFound();
            }

            var logisticalTransporter = await _context.LogisticalTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logisticalTransporter == null)
            {
                return NotFound();
            }

            return PartialView(logisticalTransporter);
            //return View(logisticalTransporter);
        }

        // POST: Management/LogisticalTransporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LogisticalTransporters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LogisticalTransporters'  is null.");
            }
            var logisticalTransporter = await _context.LogisticalTransporters.FindAsync(id);
            if (logisticalTransporter != null)
            {
                _context.LogisticalTransporters.Remove(logisticalTransporter);
            }
            
            await _context.SaveChangesAsync();
            TempData["Delete"] = "Logistical Transporter Deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool LogisticalTransporterExists(int id)
        {
          return _context.LogisticalTransporters.Any(e => e.Id == id);
        }
    }
}
