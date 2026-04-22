using System;
using System.Collections.Generic;
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
                var response = await _httpClient.GetFromJsonAsync<List<Poi>>($"{_baseUrl}/PoiApi", options);

                if (response != null)
                {
                    // ĐỒNG BỘ: Ép toàn bộ đường dẫn ảnh thành dạng tuyệt đối (http://...)
                    string serverHost = _baseUrl.Replace("/api", "");

                    foreach (var poi in response)
                    {
                        if (!string.IsNullOrEmpty(poi.ImageUrl) && !poi.ImageUrl.StartsWith("http"))
                        {
                            string separator = poi.ImageUrl.StartsWith("/") ? "" : "/";
                            poi.ImageUrl = $"{serverHost}{separator}{poi.ImageUrl}";
                        }
                    }
                    return response;
                }

                return new List<Poi>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI API]: {ex.Message}");
                return new List<Poi>();
            }
        }
    }
}