using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models.ViewModels
{
    public class BookingCreateViewModel
    {
        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }
        public Hotel? Hotel { get; set; }

        public int? SelectedRoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 20, ErrorMessage = "Số lượng khách không hợp lệ")]
        public int GuestsCount { get; set; } = 2;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại di động Việt Nam không hợp lệ (10 chữ số)")]
        public string CustomerPhone { get; set; } = string.Empty;

        public string? SpecialRequests { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "VietQR"; // VietQR, VNPay, Momo, Cash

        // Selected Extra Services: ServiceId -> Quantity
        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public Dictionary<int, int> ServiceQuantities { get; set; } = new Dictionary<int, int>();

        // All available services to choose from
        public List<ExtraService> AvailableServices { get; set; } = new List<ExtraService>();

        public int TotalNights => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);
        public decimal RoomTotalCost => (RoomType?.BasePrice ?? 0) * TotalNights;
    }

    public class BookingConfirmationViewModel
    {
        public Booking Booking { get; set; } = null!;
        public string VietQrUrl { get; set; } = string.Empty;
        public string BankName { get; set; } = "MB Bank (Ngân Hàng Quân Đội)";
        public string AccountNumber { get; set; } = "999988886666";
        public string AccountName { get; set; } = "HOTEL HOMESTAY SYSTEM";
        public string TransferContent { get; set; } = string.Empty;
    }

    public class BookingLookupViewModel
    {
        public string? BookingCode { get; set; }
        public string? CustomerPhone { get; set; }
        public Booking? BookingResult { get; set; }
        public bool HasSearched { get; set; } = false;
        public string? ErrorMessage { get; set; }
    }
}
