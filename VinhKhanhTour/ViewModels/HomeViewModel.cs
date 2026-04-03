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

        public HomeViewModel()
        {
            FeaturedPlaces = new ObservableCollection<FoodPlace>();

            // Lấy kết nối Database từ cửa thần kỳ
            _poiRepository = MauiProgram.Services.GetService<PoiRepository>();

            // Load dữ liệu lần đầu
            _ = LoadDataAsync();

            // Lắng nghe nếu người dùng bấm Đổi ngôn ngữ ở SettingsPage -> Tự load lại
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
            // Truy vấn lấy toàn bộ dữ liệu từ SQLite
            var pois = await _poiRepository.GetAllPoisAsync();

            // Vì đang tải dữ liệu ngầm, phải đẩy kết quả lên UI Thread để tránh crash
            MainThread.BeginInvokeOnMainThread(() =>
            {
                FeaturedPlaces.Clear();

                // Tạm thời lấy 3 quán đầu tiên làm danh sách "Địa điểm nổi bật"
                foreach (var poi in pois.Take(3))
                {
                    FeaturedPlaces.Add(new FoodPlace
                    {
                        Id = poi.Id.ToString(),
                        // Sức mạnh của OOP: poi.DisplayName tự động trả về đúng ngôn ngữ hiện tại!
                        Name = poi.DisplayName,
                        Address = "Vĩnh Khánh, Q4",
                        Rating = 4.8,
                        ReviewCount = 150 + poi.Id * 5, // Random tí số liệu cho đẹp
                        ImageUrl = poi.ImageUrl,
                        NarrationText = poi.DisplayTtsScript
                    });
                }
            });
        }
    }
}