using System.Collections.Generic;
using VinhKhanhTour.Models;

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
                    new FoodItem { Name = "Mì Vịt Tiềm", Price = "110.000đ", ImageUrl = "featured_food_2.jpg" },
                    new FoodItem { Name = "Sủi Cảo Tôm Thịt", Price = "65.000đ", ImageUrl = "featured_food_1.jpg" },
                    new FoodItem { Name = "Mì Hoành Thánh Xá Xíu", Price = "60.000đ", ImageUrl = "oc_vu_dish.jpg" }
                };
            }

            // Thực đơn cho Ốc Oanh
            if (placeName.Contains("Ốc Oanh"))
            {
                return new List<FoodItem>
                {
                    new FoodItem { Name = "Ốc Hương Rang Muối", Price = "120.000đ", ImageUrl = "oc_huong_bo_toi.jpg" },
                    new FoodItem { Name = "Càng Ghẹ Rang Me", Price = "150.000đ", ImageUrl = "featured_food_1.jpg" },
                    new FoodItem { Name = "Nghêu Hấp Thái", Price = "80.000đ", ImageUrl = "featured_food_2.jpg" }
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