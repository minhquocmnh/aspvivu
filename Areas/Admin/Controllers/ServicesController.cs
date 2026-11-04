using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.ExtraServices
                .Include(s => s.BookingServices)
                .OrderByDescending(s => s.IsActive)
                .ThenBy(s => s.Name)
                .ToListAsync();

            return View(services);
        }

        // GET: /Admin/Services/Create
        public IActionResult Create()
        {
            return View(new ExtraService { IsActive = true, Unit = "người / ngày", Icon = "bi-stars" });
        }

        // POST: /Admin/Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExtraService service)
        {
            if (ModelState.IsValid)
            {
                _context.ExtraServices.Add(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm dịch vụ '{service.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        // GET: /Admin/Services/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.ExtraServices.FindAsync(id);
            if (service == null) return NotFound();

            return View(service);
        }

        // POST: /Admin/Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExtraService service)
        {
            if (id != service.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật dịch vụ '{service.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        // POST: /Admin/Services/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var service = await _context.ExtraServices.FindAsync(id);
            if (service != null)
            {
                service.IsActive = !service.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã {(service.IsActive ? "kích hoạt" : "tạm ngừng")} dịch vụ '{service.Name}'.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Services/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.ExtraServices.FindAsync(id);
            if (service != null)
            {
                var isUsed = await _context.BookingServices.AnyAsync(bs => bs.ServiceId == id);
                if (isUsed)
                {
                    // Soft delete / deactivate instead of hard delete to keep booking history intact
                    service.IsActive = false;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Dịch vụ này đã có trong đơn đặt phòng của khách, hệ thống đã chuyển sang trạng thái ngưng kích hoạt để bảo toàn dữ liệu lịch sử.";
                }
                else
                {
                    _context.ExtraServices.Remove(service);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã xóa dịch vụ thành công.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
