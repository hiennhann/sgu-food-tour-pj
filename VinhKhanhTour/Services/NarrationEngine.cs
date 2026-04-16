using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;

namespace VinhKhanhTour.Services
{
    public class NarrationEngine
    {
        public static NarrationEngine Instance { get; } = new NarrationEngine();

        private string _lastPlayedPlaceId = string.Empty;
        private DateTime _lastPlayedTime = DateTime.MinValue;

        private readonly TimeSpan _cooldown = TimeSpan.FromMinutes(2);
        private CancellationTokenSource? _ttsCts;

        // Dùng chung 1 HttpClient cho toàn app để tối ưu hiệu suất kết nối mạng
        private static readonly HttpClient _httpClient = new HttpClient();

        // ==========================================
        // BƯỚC SỬA LỖI: NGỤY TRANG THÀNH GOOGLE CHROME
        // ==========================================
        static NarrationEngine()
        {
            // Bắn header này lên để Google Translate không chặn API của app
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        private NarrationEngine() { }

        public async Task PlayNarrationAsync(FoodPlace place, bool isManual = false)
        {
            if (string.IsNullOrEmpty(place?.NarrationText)) return;

            // Chống spam đọc đè liên tục
            if (!isManual && place.Id == _lastPlayedPlaceId && DateTime.Now - _lastPlayedTime < _cooldown)
            {
                Debug.WriteLine($"[NarrationEngine] Đang trong thời gian Cooldown. Bỏ qua: {place.Name}");
                return;
            }

            try
            {
                CancelCurrentNarration();
                _ttsCts = new CancellationTokenSource();

                Debug.WriteLine($"[NarrationEngine] Đã kích hoạt kịch bản: {place.Name}");
                _lastPlayedPlaceId = place.Id ?? "";
                _lastPlayedTime = DateTime.Now;

                var options = new SpeechOptions { Pitch = 1.0f, Volume = 1.0f };

                // 1. Lấy mã ngôn ngữ hiện tại của app (vd: 'en', 'fr', 'vi')
                var currentCode = LocalizationResourceManager.Instance.CurrentLanguageCode;
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                var localesList = locales?.ToList() ?? new List<Locale>();

                System.Globalization.CultureInfo cultureInfo;
                try { cultureInfo = new System.Globalization.CultureInfo(currentCode); }
                catch { cultureInfo = new System.Globalization.CultureInfo("en"); }

                var twoLetter = cultureInfo.TwoLetterISOLanguageName;
                var threeLetter = cultureInfo.ThreeLetterISOLanguageName;
                var englishName = cultureInfo.EnglishName;
                int spaceIndex = englishName.IndexOf(' ');
                var searchName = spaceIndex > 0 ? englishName.Substring(0, spaceIndex) : englishName;

                // 2. Tìm đúng giọng đọc của người bản xứ
                var selectedLocale = localesList.FirstOrDefault(l =>
                    l.Language != null && (l.Language.Equals(twoLetter, StringComparison.OrdinalIgnoreCase) || l.Language.Equals(threeLetter, StringComparison.OrdinalIgnoreCase) || l.Language.Equals(currentCode, StringComparison.OrdinalIgnoreCase)))
                    ?? localesList.FirstOrDefault(l =>
                    l.Language != null && (l.Language.StartsWith(twoLetter + "-") || l.Language.StartsWith(twoLetter + "_") || l.Language.StartsWith(threeLetter + "-") || l.Language.StartsWith(threeLetter + "_") || l.Language.StartsWith(currentCode + "-") || l.Language.StartsWith(currentCode + "_")))
                    ?? localesList.FirstOrDefault(l =>
                    l.Name != null && l.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    ?? localesList.FirstOrDefault(l =>
                    l.Id != null && l.Id.Contains(twoLetter, StringComparison.OrdinalIgnoreCase));

                if (selectedLocale != null)
                {
                    options.Locale = selectedLocale;
                }

                // ==========================================
                // 3. GỌI TRẠM DỊCH THUẬT GOOGLE (CÁCH 1)
                // ==========================================
                string textToRead = place.NarrationText;

                // Nếu không phải tiếng Việt thì mang đi dịch
                if (!currentCode.StartsWith("vi", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine($"[NarrationEngine] Đang dịch văn bản sang ngôn ngữ: {currentCode}");
                    textToRead = await TranslateTextAsync(textToRead, currentCode);
                }

                // 4. Cho máy đọc đoạn văn bản ĐÃ ĐƯỢC DỊCH
                await TextToSpeech.Default.SpeakAsync(textToRead, options, cancelToken: _ttsCts.Token);

                Debug.WriteLine($"[NarrationEngine] Đã đọc xong kịch bản: {place.Name}");
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[NarrationEngine] TTS bị hủy bỏ vì có yêu cầu mới tới.");
            }
            catch (Exception ex)
            {
                ModalErrorHandler.Instance.HandleError(ex);
            }
        }

        // ==========================================
        // HÀM GỌI API GOOGLE TRANSLATE (Miễn phí)
        // ==========================================
        private async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            try
            {
                // sl=vi (Nguồn: Tiếng Việt), tl={targetLanguage} (Đích: Ngôn ngữ app đang chọn)
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=vi&tl={targetLanguage}&dt=t&q={Uri.EscapeDataString(text)}";

                var response = await _httpClient.GetStringAsync(url);

                // Dữ liệu Google trả về bị lồng nhiều lớp ngoặc vuông, dùng JSON để bóc tách lấy chữ
                using var doc = JsonDocument.Parse(response);
                var sb = new StringBuilder();

                foreach (var element in doc.RootElement[0].EnumerateArray())
                {
                    sb.Append(element[0].GetString());
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NarrationEngine] Lỗi dịch thuật: {ex.Message}");
                // Nếu rớt mạng, lỡ lỗi thì đọc đại tiếng Việt chứ không để máy im lặng
                return text;
            }
        }

        public void CancelCurrentNarration()
        {
            if (_ttsCts?.IsCancellationRequested == false)
            {
                _ttsCts.Cancel();
                _ttsCts.Dispose();
                _ttsCts = null;
            }
        }
    }
}