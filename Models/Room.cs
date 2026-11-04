using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Số phòng không được để trống")]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        public int Floor { get; set; } = 1;

        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public bool IsActive { get; set; } = true;

        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
