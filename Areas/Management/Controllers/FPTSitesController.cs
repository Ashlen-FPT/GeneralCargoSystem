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
    public class FPTSitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FPTSitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Management/FPTSites
        public async Task<IActionResult> Index()
        {
            ViewBag.Created = TempData["Create"] as string;
            ViewBag.Updated = TempData["Update"] as string;
            ViewBag.Deleted = TempData["Delete"] as string;

            return View(await _context.FPTSites.ToListAsync());
        }

        #region RemoteValidations

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsFPTSiteExist(string siteLocation, int Id = 0)
        {

            bool isExist = _context.FPTSites.Any(x => x.SiteLocation == siteLocation && x.Id != Id);
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
        public JsonResult IsLocationIdExist(string locationId, int Id = 0)
        {

            bool isExist = _context.FPTSites.Any(x => x.LocationId == locationId && x.Id != Id);
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

        // GET: Management/FPTSites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FPTSites == null)
            {
                return NotFound();
            }

            var fPTSites = await _context.FPTSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fPTSites == null)
            {
                return NotFound();
            }

            return PartialView(fPTSites);
            //return View(fPTSites);
        }

        // GET: Management/FPTSites/Create
        public IActionResult Create()
        {
            return PartialView();
            //return View();
        }

        // POST: Management/FPTSites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,SiteLocation,LocationId")]*/ FPTSites fPTSites)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fPTSites);
                await _context.SaveChangesAsync();
                TempData["Create"] = "FPT Site Created !";
                return RedirectToAction(nameof(Index));
            }
            return View(fPTSites);
        }

        // GET: Management/FPTSites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FPTSites == null)
            {
                return NotFound();
            }

            var fPTSites = await _context.FPTSites.FindAsync(id);
            if (fPTSites == null)
            {
                return NotFound();
            }
            return PartialView(fPTSites);
            //return View(fPTSites);
        }

        // POST: Management/FPTSites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("Id,SiteLocation,LocationId")]*/ FPTSites fPTSites)
        {
            if (id != fPTSites.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fPTSites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FPTSitesExists(fPTSites.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Update"] = "FPT Site Updated !";
                return RedirectToAction(nameof(Index));
            }
            return View(fPTSites);
        }

        // GET: Management/FPTSites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FPTSites == null)
            {
                return NotFound();
            }

            var fPTSites = await _context.FPTSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fPTSites == null)
            {
                return NotFound();
            }

            return PartialView(fPTSites);
            //return View(fPTSites);
        }

        // POST: Management/FPTSites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FPTSites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FPTSites'  is null.");
            }
            var fPTSites = await _context.FPTSites.FindAsync(id);
            if (fPTSites != null)
            {
                _context.FPTSites.Remove(fPTSites);
            }
            
            await _context.SaveChangesAsync();
            TempData["Delete"] = "FPT Site Deleted !";
            return RedirectToAction(nameof(Index));
        }

        private bool FPTSitesExists(int id)
        {
          return _context.FPTSites.Any(e => e.Id == id);
        }
    }
}
