using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using VinhKhanhTour.Models; // Sử dụng FoodPlace

namespace VinhKhanhTour.Services
{
    public class NarrationEngine
    {
        // Biến Singleton để xài chung toàn app
        public static NarrationEngine Instance { get; } = new NarrationEngine();

        private string _lastPlayedPlaceId = string.Empty;
        private DateTime _lastPlayedTime = DateTime.MinValue;

        private readonly TimeSpan _cooldown = TimeSpan.FromMinutes(2);
        private CancellationTokenSource? _ttsCts;

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

                // Chạy Text-To-Speech
                await TextToSpeech.Default.SpeakAsync(place.NarrationText, options, cancelToken: _ttsCts.Token);

                Debug.WriteLine($"[NarrationEngine] Đã đọc xong kịch bản: {place.Name}");
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[NarrationEngine] TTS bị hủy bỏ vì có yêu cầu mới tới.");
            }
            catch (Exception ex)
            {
                // Dùng ModalErrorHandler để báo lỗi
                ModalErrorHandler.Instance.HandleError(ex);
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