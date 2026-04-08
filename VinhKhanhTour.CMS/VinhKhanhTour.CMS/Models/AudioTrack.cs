using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class AudioTrack
    {
        [Key]
        public int Id { get; set; }

        public int PoiId { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string AudioUrl { get; set; }
    }
}