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
    public class CommoditiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommoditiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Management/Commodities
        public async Task<IActionResult> Index()
        {
            ViewBag.Created = TempData["Create"] as string;
            ViewBag.Updated = TempData["Update"] as string;
            ViewBag.Deleted = TempData["Delete"] as string;

            return View(await _context.Commodities.ToListAsync());
        }

        // GET: Management/Commodities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Commodities == null)
            {
                return NotFound();
            }

            var commodity = await _context.Commodities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (commodity == null)
            {
                return NotFound();
            }
            return PartialView(commodity);
            //return View(commodity);
        }

        // GET: Management/Commodities/Create
        public IActionResult Create()
        {
            return PartialView();
            //return View();
        }

        // POST: Management/Commodities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,CommodityItem")]*/ Commodity commodity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(commodity);
                await _context.SaveChangesAsync();
                TempData["Create"] = "Commodity Created !";
                return RedirectToAction(nameof(Index));
            }
            return View(commodity);
        }

        // GET: Management/Commodities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Commodities == null)
            {
                return NotFound();
            }

            var commodity = await _context.Commodities.FindAsync(id);
            if (commodity == null)
            {
                return NotFound();
            }
            return PartialView(commodity);
            //return View(commodity);
        }

        // POST: Management/Commodities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("Id,CommodityItem")]*/ Commodity commodity)
        {
            if (id != commodity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commodity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommodityExists(commodity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Update"] = "Commodity Updated !";
                return RedirectToAction(nameof(Index));
            }
            return View(commodity);
        }

        // GET: Management/Commodities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Commodities == null)
            {
                return NotFound();
            }

            var commodity = await _context.Commodities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (commodity == null)
            {
                return NotFound();
            }

            return PartialView(commodity);
            //return View(commodity);
        }

        // POST: Management/Commodities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Commodities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Commodities'  is null.");
            }
            var commodity = await _context.Commodities.FindAsync(id);
            if (commodity != null)
            {
                _context.Commodities.Remove(commodity);
            }
            
            await _context.SaveChangesAsync();
            TempData["Delete"] = "Commodity Deleted !";
            return RedirectToAction(nameof(Index));
        }

        private bool CommodityExists(int id)
        {
          return _context.Commodities.Any(e => e.Id == id);
        }
    }
}
