using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.CMS.Models
{
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string KeyName { get; set; }

        [Required]
        public string TranslatedValue { get; set; }
    }
}