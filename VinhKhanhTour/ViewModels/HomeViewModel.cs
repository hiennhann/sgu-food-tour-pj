using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private List<FoodPlace> _allPlaces = new List<FoodPlace>();

        // Biến kiểm soát chống spam load data
        private bool _isLoading = false;

        public HomeViewModel()
        {
            FeaturedPlaces = new ObservableCollection<FoodPlace>();

            // Đưa vào luồng chạy ngầm để không làm đứng màn hình
            Task.Run(async () => await LoadDataAsync());

            LocalizationResourceManager.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentLanguageCode")
                {
                    Task.Run(async () => await LoadDataAsync());
                }
            };
        }

        private async Task LoadDataAsync()
        {
            // Nếu hàm đang tải dữ liệu dở dang thì từ chối gọi tiếp để tránh đụng độ
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                // BƯỚC CHỐNG LỖI: Chờ tối đa 2 giây cho đến khi hệ thống Service tiêm thành công
                int retry = 0;
                while (MauiProgram.Services == null && retry < 20)
                {
                    await Task.Delay(100);
                    retry++;
                }

                var poiRepository = MauiProgram.Services?.GetService<PoiRepository>();
                if (poiRepository == null) return;

                var pois = await poiRepository.GetAllPoisAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _allPlaces.Clear();

                    if (pois != null && pois.Any())
                    {
                        foreach (var poi in pois)
                        {
                            _allPlaces.Add(new FoodPlace
                            {
                                Id = poi.Id.ToString(),
                                // Nếu CMS trống tên hiển thị, lấy tên gốc. Nếu trống hết, để mặc định
                                Name = poi.DisplayName ?? poi.Name ?? "Quán ốc chưa có tên",
                                Address = poi.Address ?? "Khu Phố Ẩm Thực Vĩnh Khánh",
                                Rating = 4.8,
                                ReviewCount = 150 + poi.Id * 5, // Random số lượt đánh giá cho đẹp UI

                                // ĐỒNG BỘ ẢNH: Nếu CMS không có ảnh, dùng ảnh default để app không bị xấu
                                ImageUrl = string.IsNullOrEmpty(poi.ImageUrl) ? "featured_food_2.jpg" : poi.ImageUrl,

                                // ĐỒNG BỘ AUDIO: Text sẽ được nạp thẳng vào NarrationEngine
                                NarrationText = poi.DisplayTtsScript ?? $"Chào mừng bạn đến với {poi.Name}"
                            });
                        }
                    }

                    // Luôn gọi bộ lọc mặc định để render danh sách
                    FilterByCategory("Tất cả");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LỖI TẢI TRANG CHỦ]: {ex.Message}");
                // Thêm 3 dòng này để App tự bật thông báo lỗi lên màn hình:
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Báo Lỗi Dữ Liệu", ex.Message, "OK");
                });
            }
            finally
            {
                // Kết thúc quá trình tải thì mở khóa lại
                _isLoading = false;
            }
        }

        public void FilterByCategory(string category)
        {
            FeaturedPlaces.Clear();

            var filteredList = _allPlaces.Where(p =>
            {
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