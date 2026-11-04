using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;

namespace HotelBookingSystem.Services
{
    public interface IBookingService
    {
        Task<(bool Success, string Message, Booking? Booking)> CreateBookingAsync(BookingCreateViewModel model);
        Task<Booking?> GetBookingByCodeAsync(string bookingCode);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int bookingId, BookingStatus status, PaymentStatus? paymentStatus = null);
        Task<bool> CancelBookingAsync(int bookingId, string reason = "");
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
    }
}
