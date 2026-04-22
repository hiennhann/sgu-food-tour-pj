using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class UsageHistory
    {
        [Key]
        public int Id { get; set; }

        // Đã xóa UserEmail và thêm 2 trường dành cho thiết bị di động
        public string DeviceId { get; set; }

        public string IpAddress { get; set; }

        [Required]
        public string ActionType { get; set; }

        [Required]
        public string Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}