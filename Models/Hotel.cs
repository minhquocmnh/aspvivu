using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách sạn / homestay")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn khu vực / thành phố")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 5)]
        public int StarRating { get; set; } = 4;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
