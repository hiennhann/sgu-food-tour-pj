using SQLite;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Services
{
    public class LocalDbService
    {
        private SQLiteAsyncConnection _db;

        public LocalDbService()
        {
            // Tạo file database nằm trong bộ nhớ an toàn của điện thoại
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "VinhKhanhTourOffline.db3");
            _db = new SQLiteAsyncConnection(dbPath);

            // Tự động tạo bảng Poi nếu chưa có
            _db.CreateTableAsync<Poi>().Wait();
        }

        // 1. Lưu danh sách mới đè lên dữ liệu cũ
        public async Task SavePoisAsync(List<Poi> pois)
        {
            await _db.DeleteAllAsync<Poi>(); // Xóa sạch kho cũ
            await _db.InsertAllAsync(pois);  // Nhét hàng mới vào
        }

        // 2. Lấy dữ liệu từ kho ra (Dùng khi mất mạng)
        public async Task<List<Poi>> GetOfflinePoisAsync()
        {
            return await _db.Table<Poi>().ToListAsync();
        }
    }
}