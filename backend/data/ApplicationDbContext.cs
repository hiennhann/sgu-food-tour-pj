using Microsoft.EntityFrameworkCore;
using SGU_FOOD_TOUR_PJ.models;

namespace SGU_FOOD_TOUR_PJ.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        // Sau này thêm các DbSet cho Restaurants, Foods... ở đây
//         public DbSet<User> Users { get; set; }
public DbSet<Restaurant> Restaurants { get; set; } // Tạo thêm Model Restaurant.cs tương tự User.cs
public DbSet<Food> Foods { get; set; }
public DbSet<Narration> Narrations { get; set; }
    }
}