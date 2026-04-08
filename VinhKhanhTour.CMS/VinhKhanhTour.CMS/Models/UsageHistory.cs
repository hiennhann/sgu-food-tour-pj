using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class UsageHistory
    {
        [Key]
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public string ActionType { get; set; }

        public string Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}