using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class CheckInRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Khách hàng nào?

        [Required]
        public int PoiId { get; set; } // Đã ghé quán ốc nào?

        public DateTime CheckInTime { get; set; } = DateTime.UtcNow; // Thời gian ghé thăm

        [MaxLength(200)]
        public string? Note { get; set; } // Cảm nghĩ ngắn gọn (vd: "Ốc hương siêu ngon!")
    }
}