using SQLite;
using System.Collections.Generic;
using System.Linq; // Bắt buộc phải có để dùng hàm .Any()
using System.Threading.Tasks;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Utilities;

namespace VinhKhanhTour.Data
{
    public class PoiRepository
    {
        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public PoiRepository()
        {
        }

        // Hàm khởi tạo Database gọn nhẹ, đã xóa bỏ hoàn toàn GetSampleData và Lính gác
        private async Task InitAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<Poi>();
        }

        // Lấy danh sách tất cả các quán
        public async Task<List<Poi>> GetAllPoisAsync()
        {
            await InitAsync();

            // Kiểm tra kết nối mạng
            if (Microsoft.Maui.Networking.Connectivity.Current.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
            {
                var apiService = MauiProgram.Services.GetService<ApiService>();

                if (apiService != null)
                {
                    var poisFromApi = await apiService.GetPoisAsync();

                    if (poisFromApi != null && poisFromApi.Any())
                    {
                        await _database.DeleteAllAsync<Poi>(); // Xóa dữ liệu cũ
                        await _database.InsertAllAsync(poisFromApi); // Lưu dữ liệu mới từ CMS 
                        return poisFromApi;
                    }
                }
            }

            // Nếu không có mạng (hoặc API lỗi), lấy dữ liệu Offline đã lưu trong SQLite
            return await _database.Table<Poi>().ToListAsync();
        }

        // Xóa 1 quán
        public async Task<int> DeletePoiAsync(Poi item)
        {
            await InitAsync();
            return await _database.DeleteAsync(item);
        }
    }
}