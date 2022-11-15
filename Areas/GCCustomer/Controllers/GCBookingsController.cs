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

namespace GeneralCargoSystem.Areas.GCCustomer.Controllers
{
    [Area("GCCustomer")]
    [NoDirectAccess]
    public class GCBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GCBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GCCustomer/GCBookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GCBookings.Include(g => g.Commodity).Include(g => g.FPTSites).Include(g => g.LogisticalTransporter);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GCCustomer/GCBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GCBookings == null)
            {
                return NotFound();
            }

            var gCBooking = await _context.GCBookings
                .Include(g => g.Commodity)
                .Include(g => g.FPTSites)
                .Include(g => g.LogisticalTransporter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gCBooking == null)
            {
                return NotFound();
            }

            return View(gCBooking);
        }

        // GET: GCCustomer/GCBookings/Create
        public IActionResult Create()
        {
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem");
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "SiteLocation");
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name");
            return View();
        }

        // POST: GCCustomer/GCBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingReference,Date,FPTSiteId,VesselId,LogisticalTransporterId,Registration,Quantity,CommodityId,Name,PhoneNumber,Email,CreatedOn")] GCBooking gCBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gCBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "LocationId", gCBooking.FPTSiteId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            return View(gCBooking);
        }

        // GET: GCCustomer/GCBookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GCBookings == null)
            {
                return NotFound();
            }

            var gCBooking = await _context.GCBookings.FindAsync(id);
            if (gCBooking == null)
            {
                return NotFound();
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "LocationId", gCBooking.FPTSiteId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            return View(gCBooking);
        }

        // POST: GCCustomer/GCBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingReference,Date,FPTSiteId,VesselId,LogisticalTransporterId,Registration,Quantity,CommodityId,Name,PhoneNumber,Email,CreatedOn")] GCBooking gCBooking)
        {
            if (id != gCBooking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gCBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GCBookingExists(gCBooking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "LocationId", gCBooking.FPTSiteId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            return View(gCBooking);
        }

        // GET: GCCustomer/GCBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GCBookings == null)
            {
                return NotFound();
            }

            var gCBooking = await _context.GCBookings
                .Include(g => g.Commodity)
                .Include(g => g.FPTSites)
                .Include(g => g.LogisticalTransporter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gCBooking == null)
            {
                return NotFound();
            }

            return View(gCBooking);
        }

        // POST: GCCustomer/GCBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GCBookings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GCBookings'  is null.");
            }
            var gCBooking = await _context.GCBookings.FindAsync(id);
            if (gCBooking != null)
            {
                _context.GCBookings.Remove(gCBooking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GCBookingExists(int id)
        {
          return _context.GCBookings.Any(e => e.Id == id);
        }
    }
}
