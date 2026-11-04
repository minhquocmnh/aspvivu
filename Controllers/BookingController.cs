using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IRoomAvailabilityService _availabilityService;
        private readonly IVietQrService _vietQrService;

        public BookingController(
            ApplicationDbContext context,
            IBookingService bookingService,
            IRoomAvailabilityService availabilityService,
            IVietQrService vietQrService)
        {
            _context = context;
            _bookingService = bookingService;
            _availabilityService = availabilityService;
            _vietQrService = vietQrService;
        }

        // GET: /Booking/Create
        public async Task<IActionResult> Create(int roomTypeId, DateTime? checkIn, DateTime? checkOut, int? guests, int? roomId)
        {
            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId);

            if (roomType == null)
            {
                return RedirectToAction("Index", "Rooms");
            }

            var inDate = (checkIn.HasValue && checkIn.Value >= DateTime.Today) ? checkIn.Value.Date : DateTime.Today;
            var outDate = (checkOut.HasValue && checkOut.Value > inDate) ? checkOut.Value.Date : inDate.AddDays(1);
            var guestsCount = (guests.HasValue && guests.Value > 0) ? guests.Value : 2;

            // Check availability
            var availableRooms = await _availabilityService.GetAvailableRoomsForTypeAsync(roomTypeId, inDate, outDate);
            if (!availableRooms.Any())
            {
                TempData["ErrorMessage"] = "Rất tiếc! Loại phòng này đã hết phòng trống trong khoảng thời gian đã chọn.";
                return RedirectToAction("Details", "Rooms", new { id = roomTypeId, checkIn = inDate, checkOut = outDate });
            }

            var extraServices = await _context.ExtraServices.Where(s => s.IsActive).ToListAsync();

            var model = new BookingCreateViewModel
            {
                RoomTypeId = roomTypeId,
                RoomType = roomType,
                Hotel = roomType.Hotel,
                SelectedRoomId = roomId ?? availableRooms.First().Id,
                CheckInDate = inDate,
                CheckOutDate = outDate,
                GuestsCount = Math.Min(guestsCount, roomType.Capacity),
                AvailableServices = extraServices,
                PaymentMethod = "VietQR"
            };

            // If user is logged in, auto-fill contact info
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    model.CustomerName = user.FullName;
                    model.CustomerEmail = user.Email;
                    model.CustomerPhone = user.Phone;
                }
            }

            return View(model);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RoomType = await _context.RoomTypes.Include(rt => rt.Hotel).FirstOrDefaultAsync(rt => rt.Id == model.RoomTypeId);
                model.Hotel = model.RoomType?.Hotel;
                model.AvailableServices = await _context.ExtraServices.Where(s => s.IsActive).ToListAsync();
                return View(model);
            }

            var (success, message, booking) = await _bookingService.CreateBookingAsync(model);

            if (!success || booking == null)
            {
                ModelState.AddModelError(string.Empty, message);
                model.RoomType = await _context.RoomTypes.Include(rt => rt.Hotel).FirstOrDefaultAsync(rt => rt.Id == model.RoomTypeId);
                model.Hotel = model.RoomType?.Hotel;
                model.AvailableServices = await _context.ExtraServices.Where(s => s.IsActive).ToListAsync();
                return View(model);
            }

            // If payment method is Cash, go directly to Confirmation
            if (model.PaymentMethod == "Cash")
            {
                TempData["SuccessMessage"] = "Đặt phòng thành công! Quý khách sẽ thanh toán trực tiếp khi nhận phòng.";
                return RedirectToAction(nameof(Confirmation), new { code = booking.BookingCode });
            }

            // Otherwise go to payment gateway page (VietQR / VNPay simulation)
            return RedirectToAction(nameof(Payment), new { code = booking.BookingCode });
        }

        // GET: /Booking/Payment?code=...
        public async Task<IActionResult> Payment(string code)
        {
            var booking = await _bookingService.GetBookingByCodeAsync(code);
            if (booking == null)
            {
                return NotFound();
            }

            if (booking.PaymentStatus == PaymentStatus.Paid)
            {
                return RedirectToAction(nameof(Confirmation), new { code = booking.BookingCode });
            }

            var vietQrUrl = _vietQrService.GenerateVietQrUrl(booking.TotalAmount, booking.BookingCode, booking.CustomerName);

            var model = new BookingConfirmationViewModel
            {
                Booking = booking,
                VietQrUrl = vietQrUrl,
                TransferContent = $"THANH TOAN {booking.BookingCode}"
            };

            return View(model);
        }

        // POST: /Booking/SimulatePaymentSuccess
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulatePaymentSuccess(string code, string method)
        {
            var booking = await _bookingService.GetBookingByCodeAsync(code);
            if (booking == null)
            {
                return NotFound();
            }

            await _bookingService.UpdateStatusAsync(booking.Id, BookingStatus.Confirmed, PaymentStatus.Paid);
            booking.PaymentMethod = method;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thanh toán thành công qua {method}! Đơn đặt phòng của quý khách đã được xác nhận.";
            return RedirectToAction(nameof(Confirmation), new { code = booking.BookingCode });
        }

        // GET: /Booking/Confirmation?code=...
        public async Task<IActionResult> Confirmation(string code)
        {
            var booking = await _bookingService.GetBookingByCodeAsync(code);
            if (booking == null)
            {
                return NotFound();
            }

            var vietQrUrl = _vietQrService.GenerateVietQrUrl(booking.TotalAmount, booking.BookingCode, booking.CustomerName);

            var model = new BookingConfirmationViewModel
            {
                Booking = booking,
                VietQrUrl = vietQrUrl,
                TransferContent = $"THANH TOAN {booking.BookingCode}"
            };

            return View(model);
        }

        // GET: /Booking/Lookup
        public async Task<IActionResult> Lookup(string? bookingCode, string? phone)
        {
            var model = new BookingLookupViewModel
            {
                BookingCode = bookingCode,
                CustomerPhone = phone
            };

            if (!string.IsNullOrWhiteSpace(bookingCode))
            {
                model.HasSearched = true;
                var query = _context.Bookings
                    .Include(b => b.Room)
                        .ThenInclude(r => r!.RoomType)
                    .Include(b => b.Room)
                        .ThenInclude(r => r!.Hotel)
                    .Include(b => b.BookingServices)
                        .ThenInclude(bs => bs.Service)
                    .AsQueryable();

                var code = bookingCode.Trim().ToUpper();
                query = query.Where(b => b.BookingCode.ToUpper() == code);

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    var cleanPhone = phone.Trim();
                    query = query.Where(b => b.CustomerPhone.Contains(cleanPhone));
                }

                model.BookingResult = await query.FirstOrDefaultAsync();
                if (model.BookingResult == null)
                {
                    model.ErrorMessage = "Không tìm thấy đơn đặt phòng với thông tin đã cung cấp. Vui lòng kiểm tra lại mã đặt phòng.";
                }
            }

            return View(model);
        }

        // POST: /Booking/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string code, string reason)
        {
            var booking = await _bookingService.GetBookingByCodeAsync(code);
            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == BookingStatus.CheckedIn || booking.Status == BookingStatus.CheckedOut)
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn đặt phòng đang ở hoặc đã hoàn tất.";
                return RedirectToAction(nameof(Lookup), new { bookingCode = code });
            }

            await _bookingService.CancelBookingAsync(booking.Id, reason);
            TempData["SuccessMessage"] = "Hủy đơn đặt phòng thành công!";

            return RedirectToAction(nameof(Lookup), new { bookingCode = code });
        }
    }
}
