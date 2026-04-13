using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VinhKhanhTour.CMS.Models

{
    public class Poi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên quán")]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string DisplayTtsScript { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; } = 50;
        public int Priority { get; set; } = 1;

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string Address { get; set; } // Giữ lại Address

        [Required]
        public string Category { get; set; }
        public bool IsActive { get; set; } = true; // Mặc định là true (Đang hoạt động)
        public virtual ICollection<AudioTrack> AudioTracks { get; set; }
    }
}