using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models.ViewModels;
using System.Diagnostics;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await _context.Hotels
                .Select(h => h.City)
                .Distinct()
                .ToListAsync();

            var featuredRooms = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .Take(6)
                .ToListAsync();

            var model = new HomeViewModel
            {
                AvailableCities = cities,
                FeaturedRoomTypes = featuredRooms,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1),
                TotalHotels = await _context.Hotels.CountAsync(),
                TotalRooms = await _context.Rooms.CountAsync()
            };

            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<string> AvailableCities { get; set; } = new List<string>();
        public List<RoomType> FeaturedRoomTypes { get; set; } = new List<RoomType>();
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
    }
}
