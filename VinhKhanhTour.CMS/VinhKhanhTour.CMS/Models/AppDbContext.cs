using Microsoft.EntityFrameworkCore;

namespace VinhKhanhTour.CMS.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Poi> Pois { get; set; }
        public DbSet<AudioTrack> Audios { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<UsageHistory> UsageHistories { get; set; }
        public DbSet<Tour> Tours { get; set; }
    }
}