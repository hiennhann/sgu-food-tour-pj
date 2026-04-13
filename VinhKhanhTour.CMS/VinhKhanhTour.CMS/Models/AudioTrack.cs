using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm thư viện này

namespace VinhKhanhTour.CMS.Models
{
    public class AudioTrack
    {
        [Key]
        public int Id { get; set; }

        public int PoiId { get; set; }

        // Bổ sung 2 dòng này để tạo Khóa Ngoại:
        [ForeignKey("PoiId")]
        public virtual Poi Poi { get; set; } // Liên kết ngược về bảng Poi

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string AudioUrl { get; set; }
    }
}