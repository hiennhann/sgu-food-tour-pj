using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class Poi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên quán")]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string DisplayTtsScript { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; } = 50;
        public int Priority { get; set; } = 1;
        public string ImageUrl { get; set; }
        public string Address { get; set; } // Giữ lại Address
        public string Category { get; set; }
    }
}