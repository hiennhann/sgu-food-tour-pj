using System.Collections.ObjectModel;
using VinhKhanhTour.Models;
using VinhKhanhTour.Helpers;

namespace VinhKhanhTour.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<FoodPlace> FeaturedPlaces { get; set; }

        public HomeViewModel()
        {
            FeaturedPlaces = new ObservableCollection<FoodPlace>();
            LoadData(AppState.CurrentLanguageCode);
            AppState.LanguageChanged += () => LoadData(AppState.CurrentLanguageCode);
        }

        private void LoadData(string langCode)
        {
            FeaturedPlaces.Clear();

            switch (langCode)
            {
                case "en":
                    FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "Vu Snail (Ốc Vũ)", Address = "37 Vinh Khanh St", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "Vu Snail is one of the oldest and most famous snail restaurants in Vinh Khanh." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "Oanh Snail (Ốc Oanh)", Address = "534 Vinh Khanh St", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "Famous for its spacious seating and signature roasted snails with salt and chili." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "Teo's Scallops", Address = "102 Vinh Khanh St", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "Specializes in grilled scallops with scallion oil." });
                    break;

                case "ko":
                    FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "오크 부 (Ốc Vũ)", Address = "빈칸 거리 37번지", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "오크 부는 빈칸에서 가장 오래되고 유명한 달팽이 요리 전문점 중 하나입니다." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "오크 오안 (Ốc Oanh)", Address = "빈칸 거리 534번지", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "넓은 공간과 유명한 구운 소금 달팽이로 유명합니다." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "가리비 아저씨", Address = "빈칸 거리 102번지", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "파기름 구이 전문점입니다." });
                    break;

                case "ja":
                    FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "オック・ヴー", Address = "ヴィンカン通り37番地", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "オック・ヴーはヴィンカンで最も古く、有名なカタツムリ料理店の一つです。" });
                    FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "オック・オアン", Address = "ヴィンカン通り534番地", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "広々とした空間と名物の塩焼きカタツムリで有名です。" });
                    FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "ホタテおじさん", Address = "ヴィンカン通り102番地", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "ネギ油焼きの専門店です。" });
                    break;

                case "zh":
                    FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "武蜗牛 (Ốc Vũ)", Address = "永庆街37号", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "武蜗牛是永庆最古老、最著名的蜗牛餐厅之一。" });
                    FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "莺蜗牛 (Ốc Oanh)", Address = "永庆街534号", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "以宽敞的座位和招牌椒盐烤蜗牛而闻名。" });
                    FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "张叔扇贝", Address = "永庆街102号", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "主打葱油烤扇贝。" });
                    break;

                // Để tránh code quá dài, bạn có thể tự chèn thêm case cho "es", "fr", "de"... vào đây nhé.
                // Nếu người dùng chọn ngôn ngữ chưa được dịch quán ăn, hệ thống sẽ rơi vào `default` (tiếng Việt).

                default: // Mặc định luôn là Tiếng Việt ("vi")
                    FeaturedPlaces.Add(new FoodPlace { Id = "1", Name = "Quán Ốc Vũ", Address = "37 Vĩnh Khánh", Rating = 4.8, ReviewCount = 154, ImageUrl = "featured_food_1.jpg", NarrationText = "Quán Ốc Vũ là một trong những quán ốc lâu đời và nổi tiếng nhất tại Vĩnh Khánh." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "2", Name = "Ốc Oanh", Address = "534 Vĩnh Khánh", Rating = 4.6, ReviewCount = 320, ImageUrl = "featured_food_2.jpg", NarrationText = "Ốc Oanh nổi bật với không gian rộng rãi và món ốc hương rang muối ớt trứ danh." });
                    FeaturedPlaces.Add(new FoodPlace { Id = "3", Name = "Sò Điệp Chú Tèo", Address = "102 Vĩnh Khánh", Rating = 4.9, ReviewCount = 89, ImageUrl = "oc_vu_dish.jpg", NarrationText = "Chuyên các món nướng mỡ hành thơm nức mũi." });
                    break;
            }
        }
    }
}