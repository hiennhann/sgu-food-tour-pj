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
            // Cổng API của Web CMS
            _baseUrl = "http://192.168.1.4:5113/api";
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
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