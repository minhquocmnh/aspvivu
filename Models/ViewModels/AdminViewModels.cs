namespace HotelBookingSystem.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public int TotalBookingsCount { get; set; }
        public int PendingBookingsCount { get; set; }
        public int TodayCheckInsCount { get; set; }
        public int TodayCheckOutsCount { get; set; }
        public int TotalRoomsCount { get; set; }
        public int AvailableRoomsNow { get; set; }
        public int OccupiedRoomsNow { get; set; }
        public double OccupancyRate { get; set; }

        public List<Booking> RecentBookings { get; set; } = new List<Booking>();
        public List<MonthlyRevenueItem> MonthlyRevenues { get; set; } = new List<MonthlyRevenueItem>();
    }

    public class MonthlyRevenueItem
    {
        public string MonthLabel { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int BookingsCount { get; set; }
    }

    public class RoomRackViewModel
    {
        public DateTime TargetDate { get; set; } = DateTime.Today;
        public List<RoomRackItem> Rooms { get; set; } = new List<RoomRackItem>();
    }

    public class RoomRackItem
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public RoomStatus SystemStatus { get; set; }
        public string CurrentStatusText { get; set; } = string.Empty; // Trống, Đang ở, Đã đặt hôm nay, Bảo trì
        public string StatusColorClass { get; set; } = string.Empty; // success, danger, warning, secondary
        public Booking? CurrentBooking { get; set; }
    }
}
