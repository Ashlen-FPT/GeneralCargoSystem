﻿using System;
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
using Microsoft.AspNetCore.Identity.UI.Services;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using System.Text;

namespace GeneralCargoSystem.Areas.GCCustomer.Controllers
{
    [Area("GCCustomer")]
    //[NoDirectAccess]
    public class GCBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public GCBookingsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        #region RemoteValidations

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsRegistrationExist(string registration, int Id = 0)
        {

            bool isExist = _context.GCBookings.Any(x => x.Registration == registration && x.Id != Id);
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
        // GET: GCCustomer/GCBookings
        public async Task<IActionResult> Index(DateTime filterDate)
        {

            string fdate = "0001/01/01";
            var applicationDbContext = _context.GCBookings.Include(g => g.Commodity).Include(g => g.FPTSites).Include(g => g.LogisticalTransporter);

            if (filterDate != Convert.ToDateTime(fdate))
            {
                ViewBag.FDate = filterDate.ToString("yyyy-MM-dd");
                var filteredList = await applicationDbContext.Where(x => x.Date == filterDate).ToListAsync();
                return View(filteredList);

            }
            ViewBag.Updated = TempData["Update"] as string;
            ViewBag.Deleted = TempData["Delete"] as string;
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult InitiateBooking(DateTime queryDate)
        {
            var status_1 = _context.GCBookings.Where(x => x.Date == queryDate && x.Time == "01:00").ToList();
            var status_2 = _context.GCBookings.Where(x => x.Date == queryDate && x.Time == "02:00").ToList();
            string date = "0001/01/01";

            ViewBag.Status1 = status_1;
            ViewBag.Status2 = status_2;

            if (queryDate != Convert.ToDateTime(date))
            {
                ViewBag.TDate = queryDate.ToString("yyyy-MM-dd");
            }

            return View();
        }

        public IActionResult SpecificBooking(DateTime sDate, string sTime)
        {
            var specificBooking = _context.GCBookings.Where(x => x.Date == sDate && x.Time == sTime).Include(g => g.Commodity).Include(g => g.FPTSites).Include(g => g.LogisticalTransporter);
            ViewBag.Time = sTime;
            return PartialView(specificBooking.ToList());
        }

        public IActionResult Booking(DateTime bDate, string bTime)
        {
            GCBooking book = new GCBooking();

            book.Time = bTime;
            book.Date = bDate;

            TempData["Date"] = bDate;
            TempData["Time"] = bTime;

            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem");
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "SiteLocation");
            ViewData["VesselId"] = new SelectList(_context.Vessels, "Id", "VesselName");
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name");

            return PartialView(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Booking(GCBooking gCBooking)
        {
            string referenceGeneration = new Random().Next(1000, 9999).ToString();
            //    generatedRef = Convert.ToString(num + 100).PadRight(4, '0');
            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;

            if (gCBooking != null)
            {
                gCBooking.Date = (DateTime)TempData["Date"]!;
                gCBooking.Time = TempData["Time"]!.ToString()!;
                gCBooking.CreatedBy = findUsername;
                gCBooking.CreatedOn = DateTime.Now;
                gCBooking.BookingReference = "GC-" + referenceGeneration + gCBooking.Date.ToString("ddMMyy");

                _context.Add(gCBooking);
                await _context.SaveChangesAsync();

                var transport = _context.LogisticalTransporters.Where(x => x.Id == gCBooking.LogisticalTransporterId).FirstOrDefault()!.Name;
                var FPTsite = _context.FPTSites.Where(x => x.Id == gCBooking.FPTSiteId).FirstOrDefault()!.SiteLocation;
                var commodity = _context.Commodities.Where(x => x.Id == gCBooking.CommodityId).FirstOrDefault()!.CommodityItem;

                //Email Notification - New Booking
                await _emailSender.SendEmailAsync(email, $"{"General Cargo Booking : " + gCBooking.Date.ToString("dd MMMM yyyy") + " AT " + gCBooking.Time}",

                    $"Booking Reference : <text> {gCBooking.BookingReference}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Booked Date : <text> {gCBooking.Date.ToString("dd MMMM yyyy")}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Booked Time : <text> {gCBooking.Time}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Booked For : <text> {transport + " - " + " Registration : " + gCBooking.Registration}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"FPT Facility  : <text> {FPTsite}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Commodity & Quantity : <text> {commodity + " , " + gCBooking.Quantity}</text>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Addtional Comments : <text>{gCBooking.Comments}</text>");

                return RedirectToAction(nameof(BookingDetails), new { id = gCBooking.Id });
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking!.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "SiteLocation", gCBooking.FPTSiteId);
            ViewData["VesselId"] = new SelectList(_context.Vessels, "Id", "VesselName", gCBooking.VesselId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            return View(gCBooking);
        }

        //TODO : PDF Generation
        public IActionResult GCBookingDetails()
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf))
            {

                doc.Add(new Paragraph("Test"));

                doc.Close();
                doc.Flush();
                pdfBytes = stream.ToArray();
            }
            return new FileContentResult(pdfBytes, "application/pdf");
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
                .Include(g => g.Vessels)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gCBooking == null)
            {
                return NotFound();
            }

            return PartialView(gCBooking);
        }

        public async Task<IActionResult> BookingDetails(int? id)
        {

            if (id == null || _context.GCBookings == null)
            {
                return NotFound();
            }

            var gCBooking = await _context.GCBookings
                .Include(g => g.Commodity)
                .Include(g => g.FPTSites)
                .Include(g => g.LogisticalTransporter)
                .Include(g => g.Vessels)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gCBooking == null)
            {
                return NotFound();
            }

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
            TempData["BookRef"] = gCBooking!.BookingReference;
            TempData["Date"] = gCBooking!.Date;
            TempData["Time"] = gCBooking!.Time;
            TempData["CreatedOn"] = gCBooking!.CreatedOn;
            TempData["CreatedBy"] = gCBooking!.CreatedBy;
            if (gCBooking == null)
            {
                return NotFound();
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "SiteLocation", gCBooking.FPTSiteId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            ViewData["VesselId"] = new SelectList(_context.Vessels, "Id", "VesselName", gCBooking.VesselId);
            return PartialView(gCBooking);
        }

        // POST: GCCustomer/GCBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,/* [Bind("Id,BookingReference,Date,FPTSiteId,VesselId,LogisticalTransporterId,Registration,Quantity,CommodityId,Name,PhoneNumber,Email,CreatedOn")]*/ GCBooking gCBooking)
        {
            var email = User.Identity!.Name!;
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == email).FirstOrDefault()!.FirstName;

            if (id != gCBooking.Id)
            {
                return NotFound();
            }

            if (gCBooking != null)
            {
                try
                {
                    gCBooking.BookingReference = (string)TempData["BookRef"]!;
                    gCBooking.Date = (DateTime)TempData["Date"]!;
                    gCBooking.Time = (string)TempData["Time"]!;
                    gCBooking.CreatedOn = (DateTime)TempData["CreatedOn"]!;
                    gCBooking.CreatedBy = (string)TempData["CreatedBy"]!;

                    _context.Update(gCBooking);
                    await _context.SaveChangesAsync();

                    var transport = _context.LogisticalTransporters.Where(x => x.Id == gCBooking.LogisticalTransporterId).FirstOrDefault()!.Name;
                    var FPTsite = _context.FPTSites.Where(x => x.Id == gCBooking.FPTSiteId).FirstOrDefault()!.SiteLocation;
                    var commodity = _context.Commodities.Where(x => x.Id == gCBooking.CommodityId).FirstOrDefault()!.CommodityItem;

                    //Email Notification - Updated Booking
                    await _emailSender.SendEmailAsync(email, $"{"Updated ~ "+DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss")+" - General Cargo Booking : " + gCBooking.Date.ToString("dd MMMM yyyy") + " AT " + gCBooking.Time}",

                        $"Booking Reference : <text> {gCBooking.BookingReference}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"Booked Date : <text> {gCBooking.Date.ToString("dd MMMM yyyy")}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"Booked Time : <text> {gCBooking.Time}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"Booked For : <text> {transport + " - " + " Registration : " + gCBooking.Registration}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"FPT Facility  : <text> {FPTsite}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"Commodity & Quantity : <text> {commodity + " , " + gCBooking.Quantity}</text>" +
                        $"<br/>" +
                        $"<br/>" +
                        $"Addtional Comments : <text>{gCBooking.Comments}</text>");
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
                TempData["Update"] = "General Cargo Booking Updated";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommodityId"] = new SelectList(_context.Commodities, "Id", "CommodityItem", gCBooking!.CommodityId);
            ViewData["FPTSiteId"] = new SelectList(_context.FPTSites, "Id", "SiteLocation", gCBooking.FPTSiteId);
            ViewData["LogisticalTransporterId"] = new SelectList(_context.LogisticalTransporters, "Id", "Name", gCBooking.LogisticalTransporterId);
            ViewData["VesselId"] = new SelectList(_context.Vessels, "Id", "VesselName", gCBooking.VesselId);
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
            TempData["Delete"] = "General Cargo Booking Deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool GCBookingExists(int id)
        {
            return _context.GCBookings.Any(e => e.Id == id);
        }
    }
}
