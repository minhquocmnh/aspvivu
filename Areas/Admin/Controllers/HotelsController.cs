using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Hotels
        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.RoomTypes)
                .ToListAsync();

            return View(hotels);
        }

        // GET: /Admin/Hotels/Create
        public IActionResult Create()
        {
            return View(new Hotel { StarRating = 4 });
        }

        // POST: /Admin/Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm khách sạn / homestay '{hotel.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(hotel);
        }

        // GET: /Admin/Hotels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: /Admin/Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel hotel)
        {
            if (id != hotel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(hotel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật thông tin '{hotel.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(hotel);
        }
    }
}
