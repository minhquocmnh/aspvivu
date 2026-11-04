using System.Web;

namespace HotelBookingSystem.Services
{
    public interface IVietQrService
    {
        string GenerateVietQrUrl(decimal amount, string bookingCode, string customerName);
    }

    public class VietQrService : IVietQrService
    {
        // Standard VietQR QuickLink format
        private const string BankBin = "970422"; // MB Bank BIN
        private const string AccountNo = "999988886666";
        private const string Template = "compact2";
        private const string AccountName = "HOTEL HOMESTAY SYSTEM";

        public string GenerateVietQrUrl(decimal amount, string bookingCode, string customerName)
        {
            var cleanDesc = $"THANH TOAN {bookingCode.ToUpper()}";
            var encodedDesc = HttpUtility.UrlEncode(cleanDesc);
            var encodedAccountName = HttpUtility.UrlEncode(AccountName);

            // Returns dynamic QR image endpoint from VietQR
            return $"https://img.vietqr.io/image/{BankBin}-{AccountNo}-{Template}.png?amount={(long)amount}&addInfo={encodedDesc}&accountName={encodedAccountName}";
        }
    }
}
