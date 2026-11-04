using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models.ViewModels
{
    public class SearchViewModel
    {
        public string? City { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 20)]
        public int Guests { get; set; } = 2;

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; } // price_asc, price_desc, capacity
        public string? Keyword { get; set; }

        // Results
        public List<RoomSearchResultItem> Results { get; set; } = new List<RoomSearchResultItem>();

        // Cities list for dropdown
        public List<string> AvailableCities { get; set; } = new List<string>();
    }

    public class RoomSearchResultItem
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public int SizeM2 { get; set; }
        public string BedType { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Amenities { get; set; } = string.Empty;
        public int AvailableRoomsCount { get; set; }
        public List<int> AvailableRoomIds { get; set; } = new List<int>();
        public decimal TotalPriceForStay { get; set; }
        public int TotalNights { get; set; }
    }
}
