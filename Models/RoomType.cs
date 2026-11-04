using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại phòng")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Sức chứa từ 1 đến 20 khách")]
        public int Capacity { get; set; } = 2;

        [Column(TypeName = "decimal(18,2)")]
        [Range(100000, 100000000, ErrorMessage = "Giá phòng hợp lệ từ 100,000 VNĐ")]
        public decimal BasePrice { get; set; } // Giá mỗi đêm

        public string ImageUrl { get; set; } = string.Empty;

        public string Amenities { get; set; } = "Wifi miễn phí, Máy lạnh, Smart TV, Ban công, Bồn tắm";

        public int SizeM2 { get; set; } = 30;

        public string BedType { get; set; } = "1 giường King size";

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
