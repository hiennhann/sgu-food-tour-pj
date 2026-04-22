namespace VinhKhanhTour.Models
{
    public class SubscriptionStatusResponse
    {
        public string Status { get; set; } // Phải khớp với JSON API: "Trial", "Premium" hoặc "Expired"
        public int HoursRemaining { get; set; }
    }
}