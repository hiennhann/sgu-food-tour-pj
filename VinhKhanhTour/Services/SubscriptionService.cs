using System.Net.Http.Json;
using VinhKhanhTour.Models;
using Microsoft.Maui.Storage;

namespace VinhKhanhTour.Services
{
    public class SubscriptionService
    {
        private readonly HttpClient _httpClient;
        private const string DEVICE_ID_KEY = "App_Device_ID";

        // URL Emulator truy cập localhost của Backend (Port 5113)
        private const string API_URL = "http://9x12w3qg-5113.asse.devtunnels.ms/api/subscription/check";

        public SubscriptionService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetOrCreateDeviceIdAsync()
        {
            var deviceId = await SecureStorage.Default.GetAsync(DEVICE_ID_KEY);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                await SecureStorage.Default.SetAsync(DEVICE_ID_KEY, deviceId);
            }
            return deviceId;
        }

        public async Task<SubscriptionStatusResponse> CheckStatusAsync(string deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(API_URL, new { deviceId = deviceId });

                if (response.IsSuccessStatusCode)
                {
                    // Lấy thẳng kết quả minh bạch từ Server!
                    return await response.Content.ReadFromJsonAsync<SubscriptionStatusResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Call API Check Subscription Error: {ex.Message}");
            }

            // Fallback an toàn: Nếu do server của bạn sập, ta cho phép người dùng Trial
            // Tránh việc app bị sập khi chưa có kết nối Internet.
            return new SubscriptionStatusResponse { Status = "Trial" };
        }
    }
}