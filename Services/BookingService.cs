using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;

namespace HotelBookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomAvailabilityService _availabilityService;

        public BookingService(ApplicationDbContext context, IRoomAvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        public async Task<(bool Success, string Message, Booking? Booking)> CreateBookingAsync(BookingCreateViewModel model)
        {
            var checkIn = model.CheckInDate.Date;
            var checkOut = model.CheckOutDate.Date;

            if (checkOut <= checkIn)
            {
                return (false, "Ngày trả phòng phải sau ngày nhận phòng ít nhất 1 đêm.", null);
            }

            if (checkIn < DateTime.Today)
            {
                return (false, "Ngày nhận phòng không thể ở quá khứ.", null);
            }

            // Find an available room
            int targetRoomId;
            if (model.SelectedRoomId.HasValue && model.SelectedRoomId.Value > 0)
            {
                var isAvail = await _availabilityService.IsRoomAvailableAsync(model.SelectedRoomId.Value, checkIn, checkOut);
                if (!isAvail)
                {
                    return (false, "Phòng này vừa được khách hàng khác đặt hoặc không khả dụng. Vui lòng chọn phòng khác.", null);
                }
                targetRoomId = model.SelectedRoomId.Value;
            }
            else
            {
                var room = await _availabilityService.GetFirstAvailableRoomAsync(model.RoomTypeId, checkIn, checkOut);
                if (room == null)
                {
                    return (false, "Rất tiếc! Hiện tại đã hết phòng trống thuộc loại này trong khoảng thời gian đã chọn.", null);
                }
                targetRoomId = room.Id;
            }

            var roomEntity = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == targetRoomId);

            if (roomEntity == null || roomEntity.RoomType == null)
            {
                return (false, "Không tìm thấy thông tin phòng hợp lệ.", null);
            }

            int totalNights = Math.Max(1, (checkOut - checkIn).Days);
            decimal roomPrice = roomEntity.RoomType.BasePrice * totalNights;
            decimal servicesPrice = 0;

            var bookingServices = new List<BookingExtraService>();

            if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
            {
                var services = await _context.ExtraServices
                    .Where(s => model.SelectedServiceIds.Contains(s.Id) && s.IsActive)
                    .ToListAsync();

                foreach (var s in services)
                {
                    int qty = 1;
                    if (model.ServiceQuantities != null && model.ServiceQuantities.TryGetValue(s.Id, out var q) && q > 0)
                    {
                        qty = q;
                    }

                    decimal totalItemPrice = s.Price * qty;
                    servicesPrice += totalItemPrice;

                    bookingServices.Add(new BookingExtraService
                    {
                        ServiceId = s.Id,
                        Quantity = qty,
                        UnitPrice = s.Price,
                        TotalPrice = totalItemPrice
                    });
                }
            }

            decimal totalAmount = roomPrice + servicesPrice;

            // Generate unique Booking Code: BK + YYYYMMDD + 4 random digits
            var random = new Random();
            var code = $"BK{DateTime.Now:yyyyMMdd}{random.Next(1000, 9999)}";

            var booking = new Booking
            {
                BookingCode = code,
                CustomerName = model.CustomerName.Trim(),
                CustomerEmail = model.CustomerEmail.Trim(),
                CustomerPhone = model.CustomerPhone.Trim(),
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalNights = totalNights,
                GuestsCount = model.GuestsCount,
                RoomId = targetRoomId,
                RoomPrice = roomPrice,
                ServicesPrice = servicesPrice,
                TotalAmount = totalAmount,
                SpecialRequests = model.SpecialRequests,
                PaymentMethod = model.PaymentMethod,
                Status = (model.PaymentMethod == "Cash") ? BookingStatus.Confirmed : BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.Now,
                BookingServices = bookingServices
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Load related entities for view
            await _context.Entry(booking).Reference(b => b.Room).Query().Include(r => r.RoomType).Include(r => r.Hotel).LoadAsync();
            await _context.Entry(booking).Collection(b => b.BookingServices).Query().Include(bs => bs.Service).LoadAsync();

            return (true, "Đặt phòng thành công!", booking);
        }

        public async Task<Booking?> GetBookingByCodeAsync(string bookingCode)
        {
            if (string.IsNullOrWhiteSpace(bookingCode)) return null;

            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.Hotel)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.BookingCode.ToLower() == bookingCode.Trim().ToLower());
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.Hotel)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(int bookingId, BookingStatus status, PaymentStatus? paymentStatus = null)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            if (paymentStatus.HasValue)
            {
                booking.PaymentStatus = paymentStatus.Value;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string reason = "")
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.Cancelled;
            if (!string.IsNullOrEmpty(reason))
            {
                booking.SpecialRequests = $"{booking.SpecialRequests} [Lý do hủy: {reason}]";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var totalRevenue = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled && b.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

            var monthRevenue = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled && b.PaymentStatus == PaymentStatus.Paid && b.CreatedAt >= firstDayOfMonth)
                .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

            var totalBookingsCount = await _context.Bookings.CountAsync();
            var pendingBookingsCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);

            var todayCheckInsCount = await _context.Bookings.CountAsync(b => b.CheckInDate.Date == today && b.Status != BookingStatus.Cancelled);
            var todayCheckOutsCount = await _context.Bookings.CountAsync(b => b.CheckOutDate.Date == today && b.Status != BookingStatus.Cancelled);

            var totalRoomsCount = await _context.Rooms.CountAsync(r => r.IsActive);
            var occupiedRoomsNow = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.CheckedIn);
            var availableRoomsNow = Math.Max(0, totalRoomsCount - occupiedRoomsNow);

            var occupancyRate = totalRoomsCount > 0 ? Math.Round((double)occupiedRoomsNow / totalRoomsCount * 100, 1) : 0;

            var recentBookings = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.Hotel)
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .ToListAsync();

            // Last 6 months revenues
            var monthlyRevenues = new List<MonthlyRevenueItem>();
            for (int i = 5; i >= 0; i--)
            {
                var targetMonthDate = today.AddMonths(-i);
                var mStart = new DateTime(targetMonthDate.Year, targetMonthDate.Month, 1);
                var mEnd = mStart.AddMonths(1);

                var rev = await _context.Bookings
                    .Where(b => b.Status != BookingStatus.Cancelled && b.PaymentStatus == PaymentStatus.Paid && b.CreatedAt >= mStart && b.CreatedAt < mEnd)
                    .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

                var count = await _context.Bookings
                    .CountAsync(b => b.CreatedAt >= mStart && b.CreatedAt < mEnd);

                monthlyRevenues.Add(new MonthlyRevenueItem
                {
                    MonthLabel = $"Tháng {targetMonthDate.Month}/{targetMonthDate.Year}",
                    Revenue = rev,
                    BookingsCount = count
                });
            }

            return new AdminDashboardViewModel
            {
                TotalRevenue = totalRevenue,
                MonthRevenue = monthRevenue,
                TotalBookingsCount = totalBookingsCount,
                PendingBookingsCount = pendingBookingsCount,
                TodayCheckInsCount = todayCheckInsCount,
                TodayCheckOutsCount = todayCheckOutsCount,
                TotalRoomsCount = totalRoomsCount,
                AvailableRoomsNow = availableRoomsNow,
                OccupiedRoomsNow = occupiedRoomsNow,
                OccupancyRate = occupancyRate,
                RecentBookings = recentBookings,
                MonthlyRevenues = monthlyRevenues
            };
        }
    }
}
