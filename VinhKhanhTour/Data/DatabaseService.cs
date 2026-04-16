using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Services
{
    public class DatabaseService
    {
        private readonly HttpClient _httpClient;

        // CHÚ Ý: Giữ đúng IP máy tính của bạn như các API khác
        private readonly string _baseUrl = "http://192.168.1.5:5113/api/AuthApi";

        public DatabaseService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        // 1. Kiểm tra Email qua API
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<bool>($"{_baseUrl}/check-email?email={email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI CHECK EMAIL]: {ex.Message}");
                return false;
            }
        }

        // 2. Đăng ký qua API
        public async Task<int> RegisterUserAsync(UserAccount newUser)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/register", newUser);
                return response.IsSuccessStatusCode ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI ĐĂNG KÝ]: {ex.Message}");
                return 0;
            }
        }

        // 3. Đăng nhập qua API
        public async Task<UserAccount> GetUserAsync(string email, string password)
        {
            try
            {
                // Đóng gói email và pass gửi lên API
                var loginData = new UserAccount { Email = email, Password = password, FullName = "login" };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    // Đăng nhập đúng -> Nhận thông tin User (Kèm FullName) từ Server về
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return await response.Content.ReadFromJsonAsync<UserAccount>(options);
                }
                return null; // Đăng nhập sai
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI ĐĂNG NHẬP]: {ex.Message}");
                return null;
            }
        }
        private readonly string _checkInUrl = "http://192.168.1.5:5113/api/CheckInApi";

        // Hàm 1: Bắn lệnh Check-in lên Server
        public async Task<bool> CheckInAsync(int userId, int poiId, string note = "")
        {
            try
            {
                var record = new { UserId = userId, PoiId = poiId, Note = note };
                var response = await _httpClient.PostAsJsonAsync(_checkInUrl, record);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Hàm 2: Lấy danh sách Nhật ký về để hiển thị
        public async Task<List<dynamic>> GetMyJourneyAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_checkInUrl}/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    // Trả về danh sách có chứa Tên quán, Thời gian, Ảnh...
                    return await response.Content.ReadFromJsonAsync<List<dynamic>>();
                }
                return new List<dynamic>();
            }
            catch
            {
                return new List<dynamic>();
            }
        }
    }
}