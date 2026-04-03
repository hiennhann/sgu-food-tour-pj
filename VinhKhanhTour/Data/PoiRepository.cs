using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using VinhKhanhTour.Models;
using VinhKhanhTour.Utilities;

namespace VinhKhanhTour.Data
{
    public class PoiRepository
    {
        private SQLiteAsyncConnection _database;

        public PoiRepository()
        {
        }

        // Hàm khởi tạo Database
        private async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            // Tạo bảng dựa trên class Poi
            await _database.CreateTableAsync<Poi>();

            // KIỂM TRA: Nếu database trống (mới cài app), tự động Seed dữ liệu mẫu vào
            var count = await _database.Table<Poi>().CountAsync();
            if (count == 0)
            {
                var initialData = Poi.GetSampleData();
                await _database.InsertAllAsync(initialData);
                System.Diagnostics.Debug.WriteLine("[Database] Đã nạp thành công dữ liệu mẫu vào SQLite!");
            }
        }

        // Lấy danh sách tất cả các quán
        public async Task<List<Poi>> GetAllPoisAsync()
        {
            await InitAsync();
            return await _database.Table<Poi>().ToListAsync();
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