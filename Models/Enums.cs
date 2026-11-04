namespace HotelBookingSystem.Models
{
    public enum RoomStatus
    {
        Available = 0,    // Sẵn sàng đón khách
        Occupied = 1,     // Đang có khách ở
        Maintenance = 2,  // Đang bảo trì / sửa chữa
        Cleaning = 3      // Đang dọn dẹp
    }

    public enum BookingStatus
    {
        Pending = 0,      // Chờ duyệt / Chờ thanh toán
        Confirmed = 1,    // Đã xác nhận
        CheckedIn = 2,    // Đã nhận phòng
        CheckedOut = 3,   // Đã trả phòng
        Cancelled = 4     // Đã hủy
    }

    public enum PaymentStatus
    {
        Unpaid = 0,       // Chưa thanh toán
        Paid = 1,         // Đã thanh toán
        Refunded = 2      // Đã hoàn tiền
    }
}
