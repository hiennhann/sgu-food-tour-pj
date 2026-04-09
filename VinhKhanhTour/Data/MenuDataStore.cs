using System.Collections.Generic;
using VinhKhanhTour.Models;

// Bổ sung: Định nghĩa trực tiếp class FoodItem ngay trong file này
namespace VinhKhanhTour.Models
{
    public class FoodItem
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
    }
}

namespace VinhKhanhTour.Data
{
    public static class MenuDataStore
    {
        public static List<FoodItem> GetMenuForPlace(string placeName)
        {
            if (string.IsNullOrEmpty(placeName)) return new List<FoodItem>();

            // Thực đơn cho Hải Ký Mì Gia
            if (placeName.Contains("Hải Ký Mì Gia"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Mì Cật Heo", Price = "60.000đ", ImageUrl = "micatheo.png" },
                    new FoodItem { Name = "Mì Xá Xíu", Price = "65.000đ", ImageUrl = "mixaxiu.png" },
                    new FoodItem { Name = "Mì Hoành Thánh", Price = "60.000đ", ImageUrl = "mihoanhthanh.png" },
                    new FoodItem { Name = "Mì Xào Bò", Price = "65.000đ", ImageUrl = "mixaobo.png" },
                    new FoodItem { Name = "Mì Xào Giòn", Price = "80.000đ", ImageUrl = "mixaogion.png" },
                    new FoodItem { Name = "Mì Xào Hải Sản", Price = "80.000đ", ImageUrl = "mixaohs.png" },
                    new FoodItem { Name = "Bò Viên Thêm", Price = "20.000đ", ImageUrl = "bovien.png" }
                };
            }

            // Thực đơn cho Ốc Oanh
            if (placeName.Contains("Ốc Oanh"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Bạch Tuột Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Ốc Phát"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Cua Hấp", Price = "200.000đ", ImageUrl = "cuahap.png" },
                    new FoodItem { Name = "Cua Sốt Trứng Muối", Price = "250.000đ", ImageUrl = "cuasottrungmuoi.png" },
                    new FoodItem { Name = "Hàu Hấp", Price = "50.000đ", ImageUrl = "hauhap.png" },
                    new FoodItem { Name = "Hàu Nướng Mỡ Hành", Price = "70.000đ", ImageUrl = "haunuongmohanh.png" },
                    new FoodItem { Name = "Ốc Xào Sa Tế", Price = "200.000đ", ImageUrl = "ocxaosate.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng Mọi", Price = "200.000đ", ImageUrl = "tomnuongmoi.png" },
                    new FoodItem { Name = "Tôm Thái Lan", Price = "250.000đ", ImageUrl = "tomthailan.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Quán Ốc Thảo Quận 4"))
            {
                return new List<FoodItem>
                {

                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Bạch Tuột Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Toàn Phương Quán"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Bạch Tuột Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png"},
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Ốc Vũ"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Bạch Tuộc Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Sò Điệp Nướng", Price = "130.000đ", ImageUrl = "sodiepnuong.png" },
                    new FoodItem { Name = "Sò Điệp Nướng Mỡ Hành", Price = "140.000đ", ImageUrl = "sodiepnuongmohanh.png" },
                    new FoodItem { Name = "Sò Dương Nướng Sa Tế", Price = "160.000đ", ImageUrl = "soduongnuongsate.png" },
                    new FoodItem { Name = "Sò Huyết Xào Sa Tế", Price = "150.000đ", ImageUrl = "sohuyetxaosate.png" },
                    new FoodItem { Name = "Sò Lông Nướng Mỡ Hành", Price = "130.000đ", ImageUrl = "solongnuongmohanh.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Hủ Tiếu Dê"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Hủ Tiếu Dê ", Price = "50.000đ", ImageUrl = "hutieude.png" },
                    new FoodItem { Name = "Hủ Tiếu Sườn Dê", Price = "70.000đ", ImageUrl = "hutieudee.png" },
                    new FoodItem { Name = "Hủ Tiếu Dê Đặc Biệt", Price = "90.000đ", ImageUrl = "hutieudeee.png" }
                };
            }

            if (placeName.Contains("Lẩu Nướng Thuận Việt"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Bạch Tuột Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Ốc Đào"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Sụn Gà Chiên Mắm", Price = "110.000đ", ImageUrl = "sungachienmam.png" },
                    new FoodItem { Name = "Sườn Cay Thái Lan", Price = "220.000đ", ImageUrl = "suoncaythailan.png" },
                    new FoodItem { Name = "Thịt Xiên Bóp Thấu", Price = "25.000đ", ImageUrl = "thitxien.png" },
                    new FoodItem { Name = "Tôm Hùm Phô Mai", Price = "850.000đ", ImageUrl = "tomhum.png" },
                    new FoodItem { Name = "Tôm Nướng Mọi", Price = "230.000đ", ImageUrl = "tomnuongmoi.png" },
                    new FoodItem { Name = "Tôm Sốt Thái Lan", Price = "190.000đ", ImageUrl = "tomthailan.png" },
                    new FoodItem { Name = "Xiên Que Thập Cẩm", Price = "15.000đ", ImageUrl = "xienque.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Mì Sườn Lò Siêu"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Mì Sườn Khô", Price = "50.000đ", ImageUrl = "misuonkho.png" },
                    new FoodItem { Name = "Mì Sườn Hoành Thánh", Price = "55.000đ", ImageUrl = "misuonhoanhthanh.png" },
                    new FoodItem { Name = "Mì Cà Ri Sườn", Price = "60.000đ", ImageUrl = "micarisuon.png" },
                    new FoodItem { Name = "Mì Sườn Hầm", Price = "55.000đ", ImageUrl = "misuonham.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Ốc Anh Sáu"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Xiên Que Thập Cẩm", Price = "15.000đ", ImageUrl = "xienque.png" },
                    new FoodItem { Name = "Đặc Sản Toàn Phương Quán", Price = "350.000đ", ImageUrl = "tpq.png" },
                    new FoodItem { Name = "Tôm Sốt Thái Lan", Price = "190.000đ", ImageUrl = "tomthailan.png" },
                    new FoodItem { Name = "Tôm Nướng Mọi", Price = "230.000đ", ImageUrl = "tomnuongmoi.png" },
                    new FoodItem { Name = "Tôm Hùm Phô Mai", Price = "850.000đ", ImageUrl = "tomhum.png" },
                    new FoodItem { Name = "Thịt Xiên Bóp Thấu", Price = "25.000đ", ImageUrl = "thitxien.png" },
                    new FoodItem { Name = "Sườn Cay Thái Lan", Price = "220.000đ", ImageUrl = "suoncaythailan.png" },
                    new FoodItem { Name = "Sụn Gà Chiên Mắm", Price = "110.000đ", ImageUrl = "sungachienmam.png" },
                    new FoodItem { Name = "Sủi Cảo", Price = "65.000đ", ImageUrl = "suicao.png" },
                    new FoodItem { Name = "Sò Lông Nướng Mỡ Hành", Price = "130.000đ", ImageUrl = "solongnuongmohanh.png" },
                    new FoodItem { Name = "Sò Huyết Xào Sa Tế", Price = "150.000đ", ImageUrl = "sohuyetxaosate.png" },
                    new FoodItem { Name = "Sò Dương Nướng Sa Tế", Price = "160.000đ", ImageUrl = "soduongnuongsate.png" },
                    new FoodItem { Name = "Sò Điệp Nướng Mỡ Hành", Price = "140.000đ", ImageUrl = "sodiepnuongmohanh.png" },
                    new FoodItem { Name = "Sò Điệp Nướng", Price = "130.000đ", ImageUrl = "sodiepnuong.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Bạch Tuộc Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Bún Cá Châu Đốc"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Set Bún Cá Châu Đốc", Price = "120.000đ", ImageUrl = "sbccd.png" },
                    new FoodItem { Name = "Bún Cá Đặc Biệt", Price = "45.000đ", ImageUrl = "bcdb.png" },
                    new FoodItem { Name = "Bún Cá Châu Đốc (Tô Thường)", Price = "35.000đ", ImageUrl = "bccdxx.png" },
                    new FoodItem { Name = "Bún Cá Đầu Cá Lóc", Price = "50.000đ", ImageUrl = "bcddx.png" }
                };
            }

            if (placeName.Contains("Sushi Ko"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Maki Rong Biển", Price = "40.000đ", ImageUrl = "makirongbien.png" },
                    new FoodItem { Name = "Sashimi Cá Hồi", Price = "89.000đ", ImageUrl = "salmoncahoi.png" },
                    new FoodItem { Name = "Sashimi Tổng Hợp", Price = "150.000đ", ImageUrl = "sashimi.png" },
                    new FoodItem { Name = "Sushi Trứng (Tamago)", Price = "35.000đ", ImageUrl = "tamagotrung.png" },
                    new FoodItem { Name = "Sushi Lươn (Unagi)", Price = "95.000đ", ImageUrl = "unagiluon.png" },
                    new FoodItem { Name = "Maki Cuộn Ngược (Uramaki)", Price = "65.000đ", ImageUrl = "uramakicuonnguoc.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Chè Hà Trâm"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Chè Bưởi", Price = "25.000đ", ImageUrl = "chebuoi.png" },
                    new FoodItem { Name = "Chè Đậu Xanh", Price = "20.000đ", ImageUrl = "chedauxanh.png" },
                    new FoodItem { Name = "Chè Dừa Dầm", Price = "30.000đ", ImageUrl = "cheduadam.png" },
                    new FoodItem { Name = "Chè Khoai Dẻo", Price = "35.000đ", ImageUrl = "chekhoaideo.png" },
                    new FoodItem { Name = "Chè Khúc Bạch", Price = "35.000đ", ImageUrl = "chekhucbach.png" },
                    new FoodItem { Name = "Chè Thái", Price = "35.000đ", ImageUrl = "chethai.png" },
                    new FoodItem { Name = "Chè Thập Cẩm", Price = "25.000đ", ImageUrl = "chethapcam.png" },
                    new FoodItem { Name = "Chè Trôi Nước", Price = "20.000đ", ImageUrl = "chetroinuoc.png" }
                };
            }

            if (placeName.Contains("Bò Né Thanh Tuyền"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Bò Né Pate", Price = "45.000đ", ImageUrl = "bonebate.png" },
                    new FoodItem { Name = "Bò Né Phô Mai", Price = "55.000đ", ImageUrl = "bonephomai.png" },
                    new FoodItem { Name = "Bò Né Ốp La", Price = "40.000đ", ImageUrl = "bonetrung.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }

            if (placeName.Contains("Phá Lấu Cô Thảo"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Phá Lấu Bò", Price = "30.000đ", ImageUrl = "phalaubo.png" },
                    new FoodItem { Name = "Phá Lấu Nước Cốt Dừa", Price = "35.000đ", ImageUrl = "phalaucotdua.png" }
                };
            }

            if (placeName.Contains("Súp Cua Hằng"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Súp Cua", Price = "25.000đ", ImageUrl = "supcua.png" },
                    new FoodItem { Name = "Súp Hải Sản", Price = "35.000đ", ImageUrl = "suphaisan.png" },
                    new FoodItem { Name = "Súp Óc Heo", Price = "40.000đ", ImageUrl = "supocheo.png" },
                    new FoodItem { Name = "Súp Rau Củ", Price = "20.000đ", ImageUrl = "supraucu.png" },
                    new FoodItem { Name = "Súp Thập Cẩm", Price = "30.000đ", ImageUrl = "supthapcam.png" },
                    new FoodItem { Name = "Súp Trứng Bắc Thảo", Price = "30.000đ", ImageUrl = "suptrungbacthao.png" }
                };
            }

            if (placeName.Contains("Bún Bò Huế Chị Gái"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Bún Bò Chả Cua", Price = "45.000đ", ImageUrl = "bunbochacua.png" },
                    new FoodItem { Name = "Bún Bò Huế Đặc Biệt", Price = "60.000đ", ImageUrl = "bunbohuedacbiet.png" },
                    new FoodItem { Name = "Bún Bò Giò Heo", Price = "50.000đ", ImageUrl = "bunboogioheo.png" },
                    new FoodItem { Name = "Bún Bò Tái Nạm Gân", Price = "55.000đ", ImageUrl = "bunbotainamgan.png" }
                };
            }

            if (placeName.Contains("Ốc 35 nghìn đồng"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Sò Huyết Xào Sa Tế", Price = "150.000đ", ImageUrl = "sohuyetxaosate.png" },
                    new FoodItem { Name = "Sò Lông Nướng Mỡ Hành", Price = "130.000đ", ImageUrl = "solongnuongmohanh.png" },
                    new FoodItem { Name = "Sủi Cảo", Price = "65.000đ", ImageUrl = "suicao.png" },
                    new FoodItem { Name = "Sụn Gà Chiên Mắm", Price = "110.000đ", ImageUrl = "sungachienmam.png" },
                    new FoodItem { Name = "Sườn Cay Thái Lan", Price = "220.000đ", ImageUrl = "suoncaythailan.png" },
                    new FoodItem { Name = "Thịt Xiên Bóp Thấu", Price = "25.000đ", ImageUrl = "thitxien.png" },
                    new FoodItem { Name = "Tôm Hùm Phô Mai", Price = "850.000đ", ImageUrl = "tomhum.png" },
                    new FoodItem { Name = "Tôm Nướng Mọi", Price = "230.000đ", ImageUrl = "tomnuongmoi.png" },
                    new FoodItem { Name = "Tôm Sốt Thái Lan", Price = "190.000đ", ImageUrl = "tomthailan.png" },
                    new FoodItem { Name = "Đặc Sản Toàn Phương Quán", Price = "350.000đ", ImageUrl = "tpq.png" },
                    new FoodItem { Name = "Xiên Que Thập Cẩm", Price = "15.000đ", ImageUrl = "xienque.png" },
                    new FoodItem { Name = "Bạch Tuộc Nướng", Price = "150.000đ", ImageUrl = "bachtuotnuong.png" },
                    new FoodItem { Name = "Cà Bắp Nướng", Price = "39.000đ", ImageUrl = "cabapnuong.png" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "180.000đ", ImageUrl = "ngheuhapthai.png" },
                    new FoodItem { Name = "Mực Trứng Hấp", Price = "150.000đ", ImageUrl = "muctrunghap.png" },
                    new FoodItem { Name = "Ốc Len Xào Dừa", Price = "200.000đ", ImageUrl = "oclenxaodua.png" },
                    new FoodItem { Name = "Ốc Móng Tay Xào Rau Muống", Price = "150.000đ", ImageUrl = "ocmongtayxaoraumuong.png" },
                    new FoodItem { Name = "Tôm Nướng", Price = "250.000đ", ImageUrl = "tomnuong.png" },
                    new FoodItem { Name = "Sò Hấp", Price = "80.000đ", ImageUrl = "sohap.png" },
                    new FoodItem { Name = "Sò Lông Nướng", Price = "120.000đ", ImageUrl = "solongnuong.png" },
                    new FoodItem { Name = "Sò Điệp Nướng", Price = "130.000đ", ImageUrl = "sodiepnuong.png" },
                    new FoodItem { Name = "Sò Điệp Nướng Mỡ Hành", Price = "140.000đ", ImageUrl = "sodiepnuongmohanh.png" },
                    new FoodItem { Name = "Sò Dương Nướng Sa Tế", Price = "160.000đ", ImageUrl = "soduongnuongsate.png" },
                    new FoodItem { Name = "Coca Cola", Price = "15.000đ", ImageUrl = "coca.png" },
                    new FoodItem { Name = "Bia Heineken", Price = "25.000đ", ImageUrl = "ken.png" },
                    new FoodItem { Name = "Bia Larue", Price = "18.000đ", ImageUrl = "larue.png" },
                    new FoodItem { Name = "Pepsi", Price = "15.000đ", ImageUrl = "pepsi.png" },
                    new FoodItem { Name = "Bia Sài Gòn", Price = "18.000đ", ImageUrl = "saigon.png" },
                    new FoodItem { Name = "Nước Tăng Lực Sting", Price = "15.000đ", ImageUrl = "sting.png" },
                    new FoodItem { Name = "Bia Tiger Bạc", Price = "24.000đ", ImageUrl = "tigerbac.png" },
                    new FoodItem { Name = "Bia Tiger Nâu", Price = "22.000đ", ImageUrl = "tigernau.png" }
                };
            }
            // Thực đơn mặc định cho các quán khác
            return new List<FoodItem>
            {
                new FoodItem { Name = "Món đặc sản 1", Price = "55.000đ", ImageUrl = "featured_food_1.jpg" },
                new FoodItem { Name = "Món đặc sản 2", Price = "75.000đ", ImageUrl = "featured_food_2.jpg" }
            };
        }
    }
}