using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomAvailabilityService _availabilityService;

        public RoomsController(ApplicationDbContext context, IRoomAvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        // GET: /Admin/Rooms/Rack
        public async Task<IActionResult> Rack(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var rackItems = await _availabilityService.GetRoomRackStatusAsync(targetDate);

            var model = new RoomRackViewModel
            {
                TargetDate = targetDate,
                Rooms = rackItems
            };

            return View(model);
        }

        // GET: /Admin/Rooms
        public async Task<IActionResult> Index(int? hotelId, RoomStatus? status)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Hotel)
                .AsQueryable();

            if (hotelId.HasValue)
            {
                query = query.Where(r => r.HotelId == hotelId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var rooms = await query
                .OrderBy(r => r.Hotel!.Name)
                .ThenBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();

            ViewBag.Hotels = await _context.Hotels.ToListAsync();
            ViewBag.CurrentHotelId = hotelId;
            ViewBag.CurrentStatus = status;

            return View(rooms);
        }

        // GET: /Admin/Rooms/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name");
            ViewBag.RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name");
            return View(new Room { Status = RoomStatus.Available, IsActive = true, Floor = 1 });
        }

        // POST: /Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                // Ensure HotelId matches RoomType's HotelId if needed
                var rt = await _context.RoomTypes.FindAsync(room.RoomTypeId);
                if (rt != null)
                {
                    room.HotelId = rt.HotelId;
                }

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm phòng số {room.RoomNumber} thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", room.HotelId);
            ViewBag.RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // GET: /Admin/Rooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", room.HotelId);
            ViewBag.RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // POST: /Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var rt = await _context.RoomTypes.FindAsync(room.RoomTypeId);
                if (rt != null) room.HotelId = rt.HotelId;

                _context.Update(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật phòng {room.RoomNumber} thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", room.HotelId);
            ViewBag.RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // POST: /Admin/Rooms/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, RoomStatus status)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                room.Status = status;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Phòng {room.RoomNumber} đã chuyển sang trạng thái '{status}'.";
            }

            return RedirectToAction(nameof(Rack));
        }

        // GET: /Admin/Rooms/RoomTypes
        public async Task<IActionResult> RoomTypes()
        {
            var types = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .Include(rt => rt.Rooms)
                .ToListAsync();

            return View(types);
        }

        // GET: /Admin/Rooms/CreateRoomType
        public async Task<IActionResult> CreateRoomType()
        {
            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name");
            return View(new RoomType());
        }

        // POST: /Admin/Rooms/CreateRoomType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoomType(RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                _context.RoomTypes.Add(roomType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm loại phòng '{roomType.Name}' thành công!";
                return RedirectToAction(nameof(RoomTypes));
            }

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", roomType.HotelId);
            return View(roomType);
        }

        // GET: /Admin/Rooms/EditRoomType/5
        public async Task<IActionResult> EditRoomType(int id)
        {
            var rt = await _context.RoomTypes.FindAsync(id);
            if (rt == null) return NotFound();

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", rt.HotelId);
            return View(rt);
        }

        // POST: /Admin/Rooms/EditRoomType/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoomType(int id, RoomType roomType)
        {
            if (id != roomType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(roomType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật loại phòng '{roomType.Name}' thành công!";
                return RedirectToAction(nameof(RoomTypes));
            }

            ViewBag.Hotels = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", roomType.HotelId);
            return View(roomType);
        }
    }
}
