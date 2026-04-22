using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class DeviceSubscription
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string DeviceId { get; set; }

        public DateTime FirstLaunchDate { get; set; } = DateTime.UtcNow;

        public bool IsPaid { get; set; } = false;

        public string? PaymentReceipt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}