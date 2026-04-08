using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TourName { get; set; }

        public string Description { get; set; }

        public string PoiIds { get; set; }

        public string CoverImageUrl { get; set; }
    }
}