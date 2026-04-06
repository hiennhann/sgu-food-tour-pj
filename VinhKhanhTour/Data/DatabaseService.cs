using SQLite;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Data
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        // Hàm khởi tạo và kết nối DB
        private async Task Init()
        {
            if (_db != null)
                return;

            // Tạo đường dẫn an toàn giấu trong ruột điện thoại (Khách không thể tìm thấy file này)
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "VinhKhanhLocal.db3");

            _db = new SQLiteAsyncConnection(databasePath);

            // Tạo bảng UserAccount (Nếu bảng đã có sẵn thì lệnh này sẽ tự động bỏ qua)
            await _db.CreateTableAsync<UserAccount>();
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ ĐĂNG NHẬP / ĐĂNG KÝ
        // ==========================================

        // 1. Kiểm tra Email đã tồn tại chưa
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            await Init();
            var user = await _db.Table<UserAccount>().Where(u => u.Email == email).FirstOrDefaultAsync();
            return user != null; // Trả về true nếu đã có người xài email này
        }

        // 2. Đăng ký (Thêm User mới vào DB)
        public async Task<int> RegisterUserAsync(UserAccount newUser)
        {
            await Init();
            return await _db.InsertAsync(newUser);
        }

        // 3. Đăng nhập (Tìm User có khớp Email và Password không)
        public async Task<UserAccount> GetUserAsync(string email, string password)
        {
            await Init();
            return await _db.Table<UserAccount>()
                            .Where(u => u.Email == email && u.Password == password)
                            .FirstOrDefaultAsync();
        }
    }
}