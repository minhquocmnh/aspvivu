using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models.ViewModels;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomAvailabilityService _availabilityService;

        public RoomsController(ApplicationDbContext context, IRoomAvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        // GET: /Rooms
        public async Task<IActionResult> Index(SearchViewModel criteria)
        {
            if (criteria.CheckInDate < DateTime.Today)
            {
                criteria.CheckInDate = DateTime.Today;
            }

            if (criteria.CheckOutDate <= criteria.CheckInDate)
            {
                criteria.CheckOutDate = criteria.CheckInDate.AddDays(1);
            }

            criteria.AvailableCities = await _context.Hotels
                .Select(h => h.City)
                .Distinct()
                .ToListAsync();

            criteria.Results = await _availabilityService.SearchAvailableRoomTypesAsync(criteria);

            return View(criteria);
        }

        // GET: /Rooms/Details/5
        public async Task<IActionResult> Details(int id, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == id);

            if (roomType == null)
            {
                return NotFound();
            }

            var inDate = (checkIn.HasValue && checkIn.Value >= DateTime.Today) ? checkIn.Value.Date : DateTime.Today;
            var outDate = (checkOut.HasValue && checkOut.Value > inDate) ? checkOut.Value.Date : inDate.AddDays(1);
            var guestsCount = (guests.HasValue && guests.Value > 0) ? guests.Value : 2;

            var availableRooms = await _availabilityService.GetAvailableRoomsForTypeAsync(id, inDate, outDate);
            var services = await _context.ExtraServices.Where(s => s.IsActive).ToListAsync();

            var viewModel = new RoomDetailsViewModel
            {
                RoomType = roomType,
                Hotel = roomType.Hotel!,
                CheckInDate = inDate,
                CheckOutDate = outDate,
                Guests = guestsCount,
                AvailableRoomsCount = availableRooms.Count,
                AvailableRooms = availableRooms,
                ExtraServices = services
            };

            return View(viewModel);
        }

        // AJAX API: /Rooms/CheckAvailability?roomTypeId=1&checkIn=2026-11-05&checkOut=2026-11-07
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            if (checkOut <= checkIn)
            {
                return Json(new { success = false, message = "Ngày trả phòng phải sau ngày nhận phòng." });
            }

            var availableRooms = await _availabilityService.GetAvailableRoomsForTypeAsync(roomTypeId, checkIn, checkOut);
            var nights = Math.Max(1, (checkOut.Date - checkIn.Date).Days);

            var rt = await _context.RoomTypes.FindAsync(roomTypeId);
            var basePrice = rt?.BasePrice ?? 0;
            var totalRoomPrice = basePrice * nights;

            return Json(new
            {
                success = true,
                availableCount = availableRooms.Count,
                isAvailable = availableRooms.Count > 0,
                nights = nights,
                basePrice = basePrice,
                totalRoomPrice = totalRoomPrice,
                formattedTotal = totalRoomPrice.ToString("N0") + " đ"
            });
        }
    }
}
