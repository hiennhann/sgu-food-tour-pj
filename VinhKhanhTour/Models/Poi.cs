using SQLite;

namespace VinhKhanhTour.Models
{
    public class Poi
    {
        [PrimaryKey] // Tuyệt đối KHÔNG dùng chữ "required" ở đây nhé
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        public string? DisplayTtsScript { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Radius { get; set; }

        public int Priority { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Category { get; set; }

        public bool IsActive { get; set; }
    }
}