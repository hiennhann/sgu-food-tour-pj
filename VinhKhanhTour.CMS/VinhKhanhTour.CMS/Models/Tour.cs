using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TourName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string PoiIds { get; set; }

        [Required]
        public string CoverImageUrl { get; set; }
    }
}