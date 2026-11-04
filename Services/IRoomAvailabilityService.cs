using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;

namespace HotelBookingSystem.Services
{
    public interface IRoomAvailabilityService
    {
        Task<List<RoomSearchResultItem>> SearchAvailableRoomTypesAsync(SearchViewModel criteria);
        Task<List<Room>> GetAvailableRoomsForTypeAsync(int roomTypeId, DateTime checkIn, DateTime checkOut);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        Task<Room?> GetFirstAvailableRoomAsync(int roomTypeId, DateTime checkIn, DateTime checkOut);
        Task<List<RoomRackItem>> GetRoomRackStatusAsync(DateTime targetDate);
    }
}
