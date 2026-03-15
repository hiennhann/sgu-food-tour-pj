using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGU_FOOD_TOUR_PJ.models
{
    [Table("users")]
    public class User
    {
        [Key]
        public Guid id { get; set; }
        
        [Required]
        public string phone_number { get; set; } = string.Empty;
        
        [Required]
        public string password_hash { get; set; } = string.Empty;
        
        public string full_name { get; set; } = string.Empty;
        
        public string role { get; set; } = "TOURIST"; // ADMIN, SHOP_OWNER, TOURIST
        
        public bool is_active { get; set; } = true;
        
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}