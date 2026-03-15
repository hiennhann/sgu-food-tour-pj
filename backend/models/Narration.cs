using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGU_FOOD_TOUR_PJ.models
{
    [Table("narrations")]
    public class Narration
    {
        [Key]
        public Guid id { get; set; }
        public Guid restaurant_id { get; set; }
        
        [Required]
        public string language_code { get; set; } = "vi"; // "vi", "en", "zh",...
        public string? text_content { get; set; }
        public string? audio_url { get; set; }
        public bool is_active { get; set; } = true;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}