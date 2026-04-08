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
                    Id = 3,
                    Name = "Ốc Phát",
                    DisplayName = "Ốc Phát",
                    DisplayTtsScript = "Chuyên các món ốc hấp, nướng thơm ngon, giá cả hợp lý phù hợp cho học sinh và sinh viên.",
                    Latitude = 10.761954,
                    Longitude = 106.702081,
                    Radius = 60,
                    Priority = 1,
                    ImageUrl = "oc_phat_a.png"
                },



                new Poi
                {
                    Id = 5,
                    Name = "Quán Ốc Thảo Quận 4",
                    DisplayName = "Quán Ốc Thảo Quận 4",
                    DisplayTtsScript = "Chuyên các món hải sản tươi ngon được chế biến đa dạng theo nhu cầu của thực khách.",
                    Latitude = 10.761680,
                    Longitude = 106.702354,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "octhao1.png"
                },

                new Poi
                {
                    Id = 7,
                    Name = "Hải Ký Mì Gia",
                    DisplayName = "Hải Ký Mì Gia",
                    DisplayTtsScript = "Quán bán các món của người Hoa như sủi cảo, hoành thánh, mì xá xíu,... .",
                    Latitude = 10.761427,
                    Longitude = 106.702463,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "hkmg.png"
                },

                new Poi
                {
                    Id = 8,
                    Name = "Toàn Phương Quán",
                    DisplayName = "Toàn Phương Quán",
                    DisplayTtsScript = "Quán phục vụ các món nhậu giá cả bình dân, món ăn được chế biến tại chỗ.",
                    Latitude = 10.761389,
                    Longitude = 106.702506,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "tpq.png"
                },

                new Poi
                {
                    Id = 9,
                    Name = "Ốc Vũ",
                    DisplayName = "Ốc Vũ",
                    DisplayTtsScript = "Quán phục vụ các món nhậu giá cả bình dân, món ăn được chế biến tại chỗ",
                    Latitude = 10.761391,
                    Longitude = 106.702681,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "ov.png"
                },

                new Poi
                {
                    Id = 10,
                    Name = "Hủ Tiếu Dê",
                    DisplayName = "Hủ Tiếu Dê",
                    DisplayTtsScript = "Phục vụ món hủ tiếu dê thơm ngon.",
                    Latitude = 10.761107,
                    Longitude = 106.703024,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "htd.png"
                },

                new Poi
                {
                    Id = 11,
                    Name = "Ốc Oanh",
                    DisplayName = "Ốc Oanh",
                    DisplayTtsScript = "Các món ốc thơm ngon, được chế biến trực tiếp tại quầy, giá cả phù hợp với người lao động, sinh viên.",
                    Latitude = 10.760986,
                    Longitude = 106.703257,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "ooa.png"
                },

                new Poi
                {
                    Id = 12,
                    Name = "Lẩu Nướng Thuận Việt",
                    DisplayName = "Lẩu Nướng Thuận Việt",
                    DisplayTtsScript = "Lẩu Nướng Thuận Việt chuyên phục vụ các món ăn Trung Hoa chuẩn vị, được chế biến theo công thức gia truyền từ xưa nên vẫn giữ được hương vị truyền thống vốn có.",
                    Latitude = 10.760850,
                    Longitude = 106.703500,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "lntv.png"
                },

                new Poi
                {
                    Id = 13,
                    Name = "Ốc Đào",
                    DisplayName = "Ốc Đào",
                    DisplayTtsScript = "Thương hiệu ốc nổi tiếng với cách chế biến đậm đà, không gian sạch sẽ và phục vụ chuyên nghiệp.",
                    Latitude = 10.760750,
                    Longitude = 106.703600,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "od.png"
                },

                new Poi
                {
                    Id = 14,
                    Name = "Mì Sườn Lò Siêu",
                    DisplayName = "Mì Sườn Lò Siêu",
                    DisplayTtsScript = "Nổi tiếng với món mì sườn non và các món tiệm mì người Hoa đặc sắc.",
                    Latitude = 10.761800,
                    Longitude = 106.702200,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "mls.png"
                },

                new Poi
                {
                    Id = 15,
                    Name = "Ốc Anh Sáu",
                    DisplayName = "Ốc Anh Sáu",
                    DisplayTtsScript = "Quán ốc bình dân với thực đơn phong phú, được nhiều người dân địa phương yêu thích.",
                    Latitude = 10.761200,
                    Longitude = 106.702900,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "oas.png"
                },

                new Poi
                {
                    Id = 16,
                    Name = "Bún Cá Châu Đốc",
                    DisplayName = "Bún Cá Châu Đốc",
                    DisplayTtsScript = "Hương vị miền Tây với nước dùng nấu từ nghệ và các loại cá tươi.",
                    Latitude = 10.760500,
                    Longitude = 106.703800,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "bccd.png"
                },

                new Poi
                {
                    Id = 17,
                    Name = "Sushi Ko",
                    DisplayName = "Sushi Ko",
                    DisplayTtsScript = "Mô hình sushi vỉa hè độc đáo, giá cả bình dân nhưng chất lượng ổn định.",
                    Latitude = 10.762100,
                    Longitude = 106.701800,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "ssk.png"
                },

                new Poi
                {
                    Id = 18,
                    Name = "Chè Hà Trâm",
                    DisplayName = "Chè Hà Trâm",
                    DisplayTtsScript = "Quán chè đa dạng từ chè nóng đến chè lạnh, là điểm dừng chân tráng miệng quen thuộc.",
                    Latitude = 10.761050,
                    Longitude = 106.703150,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "cht.png"
                },

                new Poi
                {
                    Id = 19,
                    Name = "Bò Né Thanh Tuyền",
                    DisplayName = "Bò Né Thanh Tuyền",
                    DisplayTtsScript = "Chuyên các món bò né, bò sốt vang phục vụ cho cả ăn sáng và ăn tối.",
                    Latitude = 10.762300,
                    Longitude = 106.701500,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "bntt.png"
                },

                new Poi
                {
                    Id = 20,
                    Name = "Phá Lấu Cô Thảo",
                    DisplayName = "Phá Lấu Cô Thảo",
                    DisplayTtsScript = "Quán phá lấu nổi tiếng nhất khu vực với nước cốt dừa béo ngậy và bánh mì giòn.",
                    Latitude = 10.761750,
                    Longitude = 106.702450,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "plct.png"
                },

                new Poi
                {
                    Id = 21,
                    Name = "Súp Cua Hằng",
                    DisplayName = "Súp Cua Hằng",
                    DisplayTtsScript = "Súp cua đầy đặn với nhiều topping như óc heo, trứng bắc thảo, thanh cua.",
                    Latitude = 10.761500,
                    Longitude = 106.702700,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "sch.png"
                },

                new Poi
                {
                    Id = 22,
                    Name = "Bún Bò Huế Chị Gái",
                    DisplayName = "Bún Bò Huế Chị Gái",
                    DisplayTtsScript = "Quán bún bò lâu nằm trên phố Vĩnh Khánh, hương vị đậm đà và phục vụ nhanh nhẹn.",
                    Latitude = 10.760900,
                    Longitude = 106.703350,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "bbhcg.png"
                },

                new Poi
                {
                    Id = 6,
                    Name = "Ốc 35K",
                    DisplayName = "Ốc 35 nghìn đồng",
                    DisplayTtsScript = "Các món ốc giá chỉ từ 35000 đồng, đồ ăn tươi ngon cùng với không gian thoáng mát",
                    Latitude = 10.761488,
                    Longitude = 106.702570,
                    Radius = 50,
                    Priority = 1,
                    ImageUrl = "balam.png"
                }



                
                // CẦN THÊM QUÁN MỚI? 
                // Chỉ cần copy một khối "new Poi { ... }," ở trên và dán xuống dưới này!
            };
        }
    }
}