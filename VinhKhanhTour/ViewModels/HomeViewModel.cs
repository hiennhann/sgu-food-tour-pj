using System.Collections.ObjectModel;
using VinhKhanhTour.Models;
using VinhKhanhTour.Helpers; // Dùng AppState

namespace VinhKhanhTour.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<FoodPlace> FeaturedPlaces { get; set; }

        public HomeViewModel()
        {
            FeaturedPlaces = new ObservableCollection<FoodPlace>();

            // Tải dữ liệu lần đầu tiên (Mặc định Tiếng Việt)
            LoadData(AppState.CurrentLanguageCode);

            // Đăng ký lắng nghe: Bất cứ khi nào ngôn ngữ đổi, tự động gọi lại hàm LoadData
            AppState.LanguageChanged += () => LoadData(AppState.CurrentLanguageCode);
        }

        private void LoadData(string langCode)
        {
            FeaturedPlaces.Clear(); // Xóa sạch danh sách cũ

            if (langCode == "ko") // DỮ LIỆU TIẾNG HÀN
            {
                FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "오크 부 (Ốc Vũ)", Address = "빈칸 거리 37번지", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "오크 부는 빈칸에서 가장 오래되고 유명한 달팽이 요리 전문점 중 하나입니다." });
                FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "오크 오안 (Ốc Oanh)", Address = "빈칸 거리 534번지", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "넓은 공간과 유명한 구운 소금 달팽이로 유명합니다." });
                FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "가리비 아저씨 (Sò Điệp Chú Tèo)", Address = "빈칸 거리 102번지", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "파기름 구이 전문점입니다." });
            }
            else if (langCode == "en") // DỮ LIỆU TIẾNG ANH
            {
                FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "Vu Snail (Ốc Vũ)", Address = "37 Vinh Khanh St", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "Vu Snail is one of the oldest and most famous snail restaurants in Vinh Khanh." });
                FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "Oanh Snail (Ốc Oanh)", Address = "534 Vinh Khanh St", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "Famous for its spacious seating and signature roasted snails with salt and chili." });
                FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "Teo's Scallops (Sò Điệp)", Address = "102 Vinh Khanh St", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "Specializes in grilled scallops with scallion oil." });
            }
            else // DỮ LIỆU TIẾNG VIỆT (Mặc định)
            {
                FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "Quán Ốc Vũ", Address = "37 Vĩnh Khánh", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "Quán Ốc Vũ là một trong những quán ốc lâu đời và nổi tiếng nhất tại Vĩnh Khánh." });
                FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "Ốc Oanh", Address = "534 Vĩnh Khánh", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "Ốc Oanh nổi bật với không gian rộng rãi và món ốc hương rang muối ớt trứ danh." });
                FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "Sò Điệp Chú Tèo", Address = "102 Vĩnh Khánh", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "Chuyên các món nướng mỡ hành thơm nức mũi." });
            }
        }
    }
}