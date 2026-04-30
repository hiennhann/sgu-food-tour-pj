using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinhKhanhTour.CMS.Models
{
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngôn ngữ")]
        public string LanguageCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn địa điểm (POI)")]
        public int PoiId { get; set; }

        // Liên kết (Tham chiếu) tới bảng Pois gốc để lấy tên gốc hiển thị cho dễ nhìn
        [ForeignKey("PoiId")]
        public virtual Poi Poi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên quán đã dịch")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string TtsScript { get; set; } // Kịch bản thuyết minh
    }
}