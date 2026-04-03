using SQLite; // Thêm thư viện SQLite
using System.Collections.Generic;

namespace VinhKhanhTour.Models
{
    // Lightweight POI model used by MapPage for map rendering and geofencing
    public class Poi
    {
        // Khai báo đây là Khóa chính (Primary Key) cho SQLite
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? DisplayTtsScript { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public int Priority { get; set; }

        // ĐÃ SỬA LỖI: Di chuyển ImageUrl ra ngoài hàm, nằm ngang hàng với các thuộc tính trên
        public string ImageUrl { get; set; } = string.Empty;

        // Provide sample data so MapPage can load POIs in the absence of a backend
        public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                new Poi { Id = 1, Name = "Quán Ốc A", DisplayName = "Quán Ốc A", DisplayTtsScript = "Quán Ốc A, nổi tiếng với...", Latitude = 10.762622, Longitude = 106.660172, Radius = 80, Priority = 1, ImageUrl = "oc_vu_dish.jpg" },
                new Poi { Id = 2, Name = "Quán Ốc B", DisplayName = "Quán Ốc B", DisplayTtsScript = "Quán Ốc B, nổi tiếng với...", Latitude = 10.761622, Longitude = 106.661172, Radius = 60, Priority = 1, ImageUrl = "featured_food_1.jpg" },
                new Poi { Id = 3, Name = "Quán Ốc C", DisplayName = "Quán Ốc C", DisplayTtsScript = "Quán Ốc C, nổi tiếng với...", Latitude = 10.763622, Longitude = 106.659172, Radius = 50, Priority = 1, ImageUrl = "featured_food_2.jpg" }
            };
        }
    }
}