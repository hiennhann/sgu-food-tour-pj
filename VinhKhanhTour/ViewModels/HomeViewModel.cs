
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
                                Name = poi.DisplayName ?? poi.Name ?? "Quán chưa có tên",
                                Address = poi.Address ?? "Khu Phố Ẩm Thực Vĩnh Khánh",
                                Category = poi.Category, // QUAN TRỌNG: Nạp Category từ DB vào đây
                                Rating = 4.8,
                                ReviewCount = 150 + poi.Id * 5,
                                ImageUrl = string.IsNullOrEmpty(poi.ImageUrl) ? "featured_food_2.jpg" : poi.ImageUrl,
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
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("Báo Lỗi Dữ Liệu", ex.Message, "OK");
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        // Đã nâng cấp hàm lọc để bám theo cả Category và Name, bao cả tiếng Anh lẫn Việt
        public void FilterByCategory(string category)
        {
            FeaturedPlaces.Clear();

            var filteredList = _allPlaces.Where(p =>
            {
                if (category == "Tất cả") return true;

                // Chuyển tất cả về chữ thường để dễ so sánh
                string cat = p.Category?.ToLower() ?? "";
                string name = p.Name?.ToLower() ?? "";
                // 1. Tab Ốc & Hải sản (Bao trọn 2 danh mục: Quán Ốc, Hải Sản)
                if (category == "Ốc & Hải sản")
                    return cat.Contains("quán ốc") || cat.Contains("hải sản") || cat.Contains("seafood") ||
                           name.Contains("ốc") || name.Contains("sò") || name.Contains("nghêu") || name.Contains("hải sản");

                // 2. Tab Đồ nướng (Map đúng với danh mục Đồ Nướng)
                if (category == "Đồ nướng")
                    return cat.Contains("đồ nướng") || cat.Contains("bbq") ||
                           name.Contains("nướng") || name.Contains("bbq") || name.Contains("ngói");

                // 3. Tab Đồ uống (Map đúng với danh mục Đồ Uống)
                if (category == "Đồ uống")
                    return cat.Contains("đồ uống") || cat.Contains("drink") || cat.Contains("cafe") ||
                           name.Contains("bia") || name.Contains("trà") || name.Contains("nước") || name.Contains("chè");

                // =======================================================
                // CÁC TAB MỞ RỘNG (Nếu sau này Hân thêm nút trên App)
                // =======================================================

                // 4. Tab Nhà hàng
                if (category == "Nhà hàng")
                    return cat.Contains("nhà hàng") || cat.Contains("restaurant") ||
                           name.Contains("nhà hàng") || name.Contains("quán ăn");

                // 5. Tab Ăn vặt
                if (category == "Ăn vặt")
                    return cat.Contains("ăn vặt") || cat.Contains("streetfood") ||
                           name.Contains("bánh") || name.Contains("chè") || name.Contains("xiên") || name.Contains("gỏi");

                // 6. Tab Khác
                if (category == "Khác")
                    return cat.Contains("khác");

                return false;
            }).ToList();

            foreach (var item in filteredList)
            {
                FeaturedPlaces.Add(item);
            }
        }
    }
}