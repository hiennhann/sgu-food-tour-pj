using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGU_FOOD_TOUR_PJ.models
{
    [Table("restaurants")]
    public class Restaurant
    {
        [Key]
        public Guid id { get; set; }
        
        public Guid? owner_id { get; set; }
        
        [Required]
        public string name { get; set; } = string.Empty;
        
        public string? description { get; set; }
        public string? address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int trigger_radius { get; set; } = 20;
        
        public string approval_status { get; set; } = "PENDING";
        public string? rejection_reason { get; set; }
        public string? image_url { get; set; }
        public bool is_active { get; set; } = true;
        
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("owner_id")]
        public User? Owner { get; set; }
    }
}