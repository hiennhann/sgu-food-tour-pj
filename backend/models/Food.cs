using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGU_FOOD_TOUR_PJ.models
{
    [Table("foods")]
    public class Food
    {
        [Key]
        public Guid id { get; set; }
        public Guid restaurant_id { get; set; }
        
        [Required]
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public decimal price { get; set; }
        public string? image_url { get; set; }
        public bool is_active { get; set; } = true;
    }
}