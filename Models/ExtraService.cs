using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class ExtraService
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 50000000, ErrorMessage = "Giá dịch vụ không hợp lệ")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "người / ngày"; // người/ngày, chuyến, lượt, xe/ngày

        [StringLength(50)]
        public string Icon { get; set; } = "bi-stars"; // Bootstrap icon class

        public bool IsActive { get; set; } = true;

        public ICollection<BookingExtraService> BookingServices { get; set; } = new List<BookingExtraService>();
    }
}
