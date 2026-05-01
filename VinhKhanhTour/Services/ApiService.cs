using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService()
        {
            // Dùng 10.0.2.2 để máy ảo Android kết nối được với Web CMS (localhost)
            //_baseUrl = "http://10.0.2.2:5113/api";
            _baseUrl = "https://9x12w3qg-5113.asse.devtunnels.ms/api";


            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            _httpClient.DefaultRequestHeaders.Add("X-Tunnel-Skip-AntiPhishing-Page", "true");
        }

        public async Task<List<Poi>> GetPoisAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // 1. Tải danh sách quán gốc (Tiếng Việt)
                var response = await _httpClient.GetFromJsonAsync<List<Poi>>($"{_baseUrl}/PoiApi", options);

                if (response != null && response.Any())
                {
                    string serverHost = _baseUrl.Replace("/api", "");

                    foreach (var poi in response)
                    {
                        if (!string.IsNullOrEmpty(poi.ImageUrl) && !poi.ImageUrl.StartsWith("http"))
                        {
                            string separator = poi.ImageUrl.StartsWith("/") ? "" : "/";
                            poi.ImageUrl = $"{serverHost}{separator}{poi.ImageUrl}";
                        }
                    }

                    // ==================================================
                    // 2. XỬ LÝ DỊCH THUẬT ĐỘNG TỪ CMS
                    // ==================================================
                    // Lấy mã ngôn ngữ hiện tại mà user đang chọn trên App
                    string currentLang = LocalizationResourceManager.Instance.CurrentLanguageCode;

                    // Nếu không phải tiếng Việt, tiến hành gọi API dịch
                    if (currentLang != "vi")
                    {
                        // Map sang mã chuẩn của CMS (vd: "en" -> "en-US")
                        string targetLang = currentLang switch
                        {
                            "en" => "en-US",
                            "ko" => "ko-KR",
                            "zh" => "zh-CN",
                            "ja" => "ja-JP",
                            "es" => "es-ES",
                            "fr" => "fr-FR",
                            "de" => "de-DE",
                            "ru" => "ru-RU",
                            "it" => "it-IT",
                            "pt" => "pt-PT",
                            "hi" => "hi-IN",
                            _ => "en-US" // Mặc định phòng hờ
                        };

                        try
                        {
                            // Gọi API lấy các bản dịch tương ứng
                            var translations = await _httpClient.GetFromJsonAsync<List<TranslationDto>>($"{_baseUrl}/TranslationApi?lang={targetLang}", options);

                            if (translations != null && translations.Any())
                            {
                                foreach (var poi in response)
                                {
                                    var trans = translations.FirstOrDefault(t => t.PoiId == poi.Id);
                                    if (trans != null)
                                    {
                                        // Ghi đè chữ dịch lên dữ liệu gốc
                                        poi.Name = trans.TranslatedName;
                                        poi.DisplayName = trans.TranslatedName; // Đổi luôn tên hiển thị
                                        poi.Address = trans.TranslatedAddress;
                                        poi.DisplayTtsScript = trans.TranslatedTtsScript;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[LỖI API DỊCH]: {ex.Message}");
                        }
                    }
                    // ==================================================

                    return response;
                }

                return new List<Poi>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI API LẤY POI]: {ex.Message}");
                return new List<Poi>();
            }
        }

        // THÊM HÀM NÀY VÀO TRONG CLASS ApiService
        public async Task LogAppAccessAsync(string deviceId, string deviceModel)
        {
            try
            {
                var payload = new { DeviceId = deviceId, DeviceModel = deviceModel };

                // Gọi nối tiếp vào cái _baseUrl đang chạy thành công của POI
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/UsageHistory/LogAccess", payload);

                // BẮT BỆNH: Nếu Server từ chối, nó sẽ in lỗi ra màu đỏ để ní biết ngay
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[LỖI API LỊCH SỬ]: {response.StatusCode} - {error}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[THÀNH CÔNG] Đã lưu lịch sử vào Database!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LỖI MẠNG LỊCH SỬ]: {ex.Message}");
            }
        }
    }
}