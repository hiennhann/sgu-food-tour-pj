using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class UsageHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string ActionType { get; set; }

        [Required]
        public string Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}