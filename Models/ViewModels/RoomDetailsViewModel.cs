namespace HotelBookingSystem.Models.ViewModels
{
    public class RoomDetailsViewModel
    {
        public RoomType RoomType { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
        public int AvailableRoomsCount { get; set; }
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
        public List<ExtraService> ExtraServices { get; set; } = new List<ExtraService>();
        public int TotalNights => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);
        public decimal TotalRoomPrice => RoomType.BasePrice * TotalNights;
    }
}
