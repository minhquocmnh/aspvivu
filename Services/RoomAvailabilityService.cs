using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;

namespace HotelBookingSystem.Services
{
    public class RoomAvailabilityService : IRoomAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public RoomAvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomSearchResultItem>> SearchAvailableRoomTypesAsync(SearchViewModel criteria)
        {
            var checkIn = criteria.CheckInDate.Date;
            var checkOut = criteria.CheckOutDate.Date;
            if (checkOut <= checkIn)
            {
                checkOut = checkIn.AddDays(1);
            }

            var totalNights = Math.Max(1, (checkOut - checkIn).Days);

            // 1. Get room IDs that are already booked for this date range
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            b.CheckInDate < checkOut &&
                            b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            // 2. Query available physical rooms
            var activeRoomsQuery = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Hotel)
                .Where(r => r.IsActive && r.Status == RoomStatus.Available && !bookedRoomIds.Contains(r.Id));

            // Filter by City if provided
            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                activeRoomsQuery = activeRoomsQuery.Where(r => r.Hotel!.City == criteria.City);
            }

            // Filter by Guests capacity
            if (criteria.Guests > 0)
            {
                activeRoomsQuery = activeRoomsQuery.Where(r => r.RoomType!.Capacity >= criteria.Guests);
            }

            // Filter by Keyword
            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                var kw = criteria.Keyword.Trim().ToLower();
                activeRoomsQuery = activeRoomsQuery.Where(r =>
                    r.RoomType!.Name.ToLower().Contains(kw) ||
                    r.Hotel!.Name.ToLower().Contains(kw) ||
                    r.Hotel!.City.ToLower().Contains(kw) ||
                    r.RoomType!.Amenities.ToLower().Contains(kw));
            }

            // Filter by Price range
            if (criteria.MinPrice.HasValue)
            {
                activeRoomsQuery = activeRoomsQuery.Where(r => r.RoomType!.BasePrice >= criteria.MinPrice.Value);
            }
            if (criteria.MaxPrice.HasValue)
            {
                activeRoomsQuery = activeRoomsQuery.Where(r => r.RoomType!.BasePrice <= criteria.MaxPrice.Value);
            }

            var availableRooms = await activeRoomsQuery.ToListAsync();

            // Group by RoomType to present available room types to the customer
            var grouped = availableRooms
                .GroupBy(r => r.RoomTypeId)
                .Select(g =>
                {
                    var first = g.First();
                    var rt = first.RoomType!;
                    var hotel = first.Hotel!;

                    return new RoomSearchResultItem
                    {
                        RoomTypeId = rt.Id,
                        RoomTypeName = rt.Name,
                        HotelName = hotel.Name,
                        City = hotel.City,
                        Address = hotel.Address,
                        StarRating = hotel.StarRating,
                        BasePrice = rt.BasePrice,
                        Capacity = rt.Capacity,
                        SizeM2 = rt.SizeM2,
                        BedType = rt.BedType,
                        ImageUrl = rt.ImageUrl,
                        Amenities = rt.Amenities,
                        AvailableRoomsCount = g.Count(),
                        AvailableRoomIds = g.Select(r => r.Id).ToList(),
                        TotalNights = totalNights,
                        TotalPriceForStay = rt.BasePrice * totalNights
                    };
                })
                .ToList();

            // Sorting
            grouped = criteria.SortBy switch
            {
                "price_asc" => grouped.OrderBy(x => x.BasePrice).ToList(),
                "price_desc" => grouped.OrderByDescending(x => x.BasePrice).ToList(),
                "capacity" => grouped.OrderByDescending(x => x.Capacity).ToList(),
                _ => grouped.OrderBy(x => x.BasePrice).ToList()
            };

            return grouped;
        }

        public async Task<List<Room>> GetAvailableRoomsForTypeAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var inDate = checkIn.Date;
            var outDate = checkOut.Date;
            if (outDate <= inDate) outDate = inDate.AddDays(1);

            var bookedRoomIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            b.CheckInDate < outDate &&
                            b.CheckOutDate > inDate)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Where(r => r.RoomTypeId == roomTypeId &&
                            r.IsActive &&
                            r.Status == RoomStatus.Available &&
                            !bookedRoomIds.Contains(r.Id))
                .ToListAsync();
        }

        public async Task<Room?> GetFirstAvailableRoomAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var rooms = await GetAvailableRoomsForTypeAsync(roomTypeId, checkIn, checkOut);
            return rooms.FirstOrDefault();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var inDate = checkIn.Date;
            var outDate = checkOut.Date;
            if (outDate <= inDate) outDate = inDate.AddDays(1);

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.IsActive || room.Status != RoomStatus.Available)
            {
                return false;
            }

            var query = _context.Bookings
                .Where(b => b.RoomId == roomId &&
                            b.Status != BookingStatus.Cancelled &&
                            b.CheckInDate < outDate &&
                            b.CheckOutDate > inDate);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<List<RoomRackItem>> GetRoomRackStatusAsync(DateTime targetDate)
        {
            var date = targetDate.Date;
            var nextDate = date.AddDays(1);

            var allRooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Hotel)
                .OrderBy(r => r.Hotel!.Name)
                .ThenBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();

            var activeBookings = await _context.Bookings
                .Include(b => b.BookingServices)
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            b.CheckInDate < nextDate &&
                            b.CheckOutDate > date)
                .ToListAsync();

            var result = new List<RoomRackItem>();

            foreach (var r in allRooms)
            {
                var item = new RoomRackItem
                {
                    RoomId = r.Id,
                    RoomNumber = r.RoomNumber,
                    Floor = r.Floor,
                    RoomTypeName = r.RoomType?.Name ?? "Chưa phân loại",
                    BasePrice = r.RoomType?.BasePrice ?? 0,
                    HotelName = r.Hotel?.Name ?? "Chưa rõ",
                    SystemStatus = r.Status
                };

                if (r.Status == RoomStatus.Maintenance)
                {
                    item.CurrentStatusText = "Đang bảo trì";
                    item.StatusColorClass = "secondary";
                }
                else if (r.Status == RoomStatus.Cleaning)
                {
                    item.CurrentStatusText = "Đang dọn dẹp";
                    item.StatusColorClass = "info";
                }
                else
                {
                    var booking = activeBookings.FirstOrDefault(b => b.RoomId == r.Id);
                    if (booking != null)
                    {
                        item.CurrentBooking = booking;
                        if (booking.Status == BookingStatus.CheckedIn)
                        {
                            item.CurrentStatusText = $"Đang ở: {booking.CustomerName}";
                            item.StatusColorClass = "danger";
                        }
                        else
                        {
                            item.CurrentStatusText = $"Đã đặt: {booking.CustomerName}";
                            item.StatusColorClass = "warning";
                        }
                    }
                    else
                    {
                        item.CurrentStatusText = "Sẵn sàng đón khách";
                        item.StatusColorClass = "success";
                    }
                }

                result.Add(item);
            }

            return result;
        }
    }
}
