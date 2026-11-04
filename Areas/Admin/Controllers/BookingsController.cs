using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;

        public BookingsController(ApplicationDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        // GET: /Admin/Bookings
        public async Task<IActionResult> Index(string? status, string? search, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.Hotel)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var parsedStatus))
            {
                query = query.Where(b => b.Status == parsedStatus);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b => b.BookingCode.ToLower().Contains(s) ||
                                         b.CustomerName.ToLower().Contains(s) ||
                                         b.CustomerPhone.Contains(s) ||
                                         b.CustomerEmail.ToLower().Contains(s));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate <= toDate.Value.Date);
            }

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            ViewBag.Search = search;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(bookings);
        }

        // GET: /Admin/Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Admin/Bookings/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, BookingStatus status, PaymentStatus? paymentStatus = null)
        {
            var success = await _bookingService.UpdateStatusAsync(id, status, paymentStatus);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn đặt sang '{status}'.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái đơn đặt phòng.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Admin/Bookings/QuickStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickStatus(int id, BookingStatus status)
        {
            await _bookingService.UpdateStatusAsync(id, status);
            TempData["SuccessMessage"] = $"Cập nhật thành công sang trạng thái '{status}'.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Bookings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa đơn đặt phòng thành công.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
