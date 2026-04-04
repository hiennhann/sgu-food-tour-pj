using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using VinhKhanhTour.Models;
using VinhKhanhTour.Utilities;

namespace VinhKhanhTour.Data
{
    public class PoiRepository
    {
        // 🔴 TĂNG SỐ NÀY LÊN MỖI KHI BẠN ĐỔI TỌA ĐỘ HOẶC THÊM QUÁN MỚI VÀO Poi.GetSampleData()
        private const int DATA_VERSION = 1;

        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public PoiRepository()
        {
        }

        // Hàm khởi tạo Database & Kiểm tra phiên bản
        private async Task InitAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<Poi>();

            // Dùng Name = "VK_VERSION" làm lính gác (Sentinel)
            // Dùng Priority để lưu con số phiên bản hiện tại
            var sentinel = await _database.Table<Poi>().FirstOrDefaultAsync(p => p.Name == "VK_VERSION");

            bool canSeedLai = false;

            if (sentinel == null || sentinel.Priority != DATA_VERSION)
            {
                // Nếu chưa có lính gác, hoặc lính gác báo version cũ -> Xóa sạch DB cũ
                await _database.DeleteAllAsync<Poi>();
                canSeedLai = true;
            }

            if (canSeedLai)
            {
                // 1. Nạp lính gác mới với Version mới
                await _database.InsertAsync(new Poi
                {
                    Name = "VK_VERSION",
                    Priority = DATA_VERSION,
                    DisplayName = "System Version",
                    DisplayTtsScript = "",
                    ImageUrl = ""
                });

                // 2. Bơm toàn bộ dữ liệu thật từ Poi.GetSampleData() vào DB
                var initialData = Poi.GetSampleData();
                await _database.InsertAllAsync(initialData);

                System.Diagnostics.Debug.WriteLine($"[Database] Đã cập nhật SQLite lên phiên bản {DATA_VERSION}!");
            }
        }

        // Lấy danh sách tất cả các quán
        public async Task<List<Poi>> GetAllPoisAsync()
        {
            await InitAsync();
            // CỰC KỲ QUAN TRỌNG: Lọc bỏ record lính gác ra khỏi danh sách để không hiện lên map
            return await _database.Table<Poi>()
                                  .Where(p => p.Name != "VK_VERSION")
                                  .ToListAsync();
        }

        // Lấy 1 quán theo ID
        public async Task<Poi> GetPoiAsync(int id)
        {
            await InitAsync();
            return await _database.Table<Poi>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        // Thêm hoặc Cập nhật 1 quán
        public async Task<int> SavePoiAsync(Poi item)
        {
            await InitAsync();
            if (item.Id != 0)
            {
                return await _database.UpdateAsync(item);
            }
            else
            {
                return await _database.InsertAsync(item);
            }
        }

        // Xóa 1 quán
        public async Task<int> DeletePoiAsync(Poi item)
        {
            await InitAsync();
            return await _database.DeleteAsync(item);
        }
    }
}