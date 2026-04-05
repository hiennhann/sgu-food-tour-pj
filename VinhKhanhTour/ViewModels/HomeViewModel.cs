using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Data;

namespace VinhKhanhTour.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<FoodPlace> FeaturedPlaces { get; set; }
        private readonly PoiRepository _poiRepository;

        // 🔴 BIẾN MỚI: Dùng để lưu trữ TOÀN BỘ danh sách quán ăn làm gốc
        private List<FoodPlace> _allPlaces = new List<FoodPlace>();

        public HomeViewModel()
        {
            FeaturedPlaces = new ObservableCollection<FoodPlace>();
            _poiRepository = MauiProgram.Services.GetService<PoiRepository>();

            _ = LoadDataAsync();

            LocalizationResourceManager.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentLanguageCode")
                {
                    _ = LoadDataAsync();
                }
            };
        }

        private async Task LoadDataAsync()
        {
            var pois = await _poiRepository.GetAllPoisAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _allPlaces.Clear();

                // Lấy TẤT CẢ quán ăn (Bỏ lệnh Take(3))
                foreach (var poi in pois)
                {
                    _allPlaces.Add(new FoodPlace
                    {
                        Id = poi.Id.ToString(),
                        Name = poi.DisplayName,
                        Address = "Vĩnh Khánh, Q4",
                        Rating = 4.8,
                        ReviewCount = 150 + poi.Id * 5,
                        ImageUrl = poi.ImageUrl,
                        NarrationText = poi.DisplayTtsScript
                    });
                }

                // Khi vừa mở app lên, mặc định sẽ lọc và hiển thị "Tất cả"
                FilterByCategory("Tất cả");
            });
        }

        // 🔴 HÀM MỚI: Thực hiện việc lọc quán ăn dựa theo tên Phân loại
        public void FilterByCategory(string category)
        {
            FeaturedPlaces.Clear();

            var filteredList = _allPlaces.Where(p =>
            {
                // Mẹo: Vì chúng ta chưa có cột Category trong Database, nên mình dùng tên quán để phân loại
                if (category == "Tất cả") return true;
                if (category == "Ốc & Hải sản") return p.Name.Contains("Ốc") || p.Name.Contains("Sò") || p.Name.Contains("Nghêu") || p.Name.Contains("Hải sản");
                if (category == "Đồ nướng") return p.Name.Contains("Nướng") || p.Name.Contains("BBQ") || p.Name.Contains("Ngói");
                if (category == "Đồ uống") return p.Name.Contains("Bia") || p.Name.Contains("Trà") || p.Name.Contains("Nước");

                return false;
            }).ToList();

            foreach (var item in filteredList)
            {
                FeaturedPlaces.Add(item);
            }
        }
    }
}