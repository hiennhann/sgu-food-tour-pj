using SQLite;
using System.Collections.Generic;

namespace VinhKhanhTour.Models
{
    // Lightweight POI model used by MapPage for map rendering and geofencing
    public class Poi
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Tên gốc của quán dùng trong hệ thống
        public string? Name { get; set; }

        // Tên hiển thị ra giao diện Bản đồ
        public string? DisplayName { get; set; }

        // Kịch bản sẽ được công cụ Audio đọc lên khi bước vào vùng Radar
        public string? DisplayTtsScript { get; set; }

        // Tọa độ GPS (Mở Google Maps, click chuột phải vào quán để copy)
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Bán kính kích hoạt Radar Audio (Tính bằng mét)
        public double Radius { get; set; }

        // Độ ưu tiên phát Audio (Nếu 2 vòng tròn đè lên nhau, số cao hơn sẽ được đọc)
        public int Priority { get; set; }

        // Tên file hình ảnh (Lưu trong thư mục Resources/Images, Build Action: MauiImage)
        public string ImageUrl { get; set; } = string.Empty;

        // =========================================================================
        // DANH SÁCH QUÁN ĂN (Viết theo chiều dọc để cực kỳ dễ copy, dán và sửa đổi)
        // =========================================================================
        public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                new Poi
                {
                    Id = 1,
                    Name = "Quán Ốc Vũ",
                    DisplayName = "Quán Ốc Vũ",
                    DisplayTtsScript = "Quán Ốc Vũ là một trong những quán ốc lâu đời và nổi tiếng nhất tại Vĩnh Khánh.",
                    Latitude = 10.762622,
                    Longitude = 106.660172,
                    Radius = 80,
                    Priority = 1,
                    ImageUrl = "oc_vu_dish.jpg"
                },

                new Poi
                {
                    Id = 2,
                    Name = "Ốc Oanh",
                    DisplayName = "Ốc Oanh",
                    DisplayTtsScript = "Ốc Oanh nổi bật với không gian rộng rãi và món ốc hương rang muối ớt trứ danh.",
                    Latitude = 10.761622,
                    Longitude = 106.661172,
                    Radius = 60,
                    Priority = 1,
                    ImageUrl = "featured_food_1.jpg"
                },

                new Poi
                {
                    Id = 3,
                    Name = "Sò Điệp Chú Tèo",
                    DisplayName = "Sò Điệp Chú Tèo",
                    DisplayTtsScript = "Chuyên các món nướng mỡ hành thơm nức mũi.",
                    Latitude = 10.763622,
                    Longitude = 106.659172,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "featured_food_2.jpg"
                }
                
                // CẦN THÊM QUÁN MỚI? 
                // Chỉ cần copy một khối "new Poi { ... }," ở trên và dán xuống dưới này!
            };
        }
    }
}