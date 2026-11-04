using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string BookingCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public int TotalNights { get; set; } = 1;

        public int GuestsCount { get; set; } = 2;

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RoomPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ServicesPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string? SpecialRequests { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "VietQR"; // VietQR, VNPay, Momo, Cash

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<BookingExtraService> BookingServices { get; set; } = new List<BookingExtraService>();
    }
}
