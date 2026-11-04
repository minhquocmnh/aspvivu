using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class BookingExtraService
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int ServiceId { get; set; }
        public ExtraService? Service { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
