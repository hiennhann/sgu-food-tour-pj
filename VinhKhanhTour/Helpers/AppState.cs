using System;

namespace VinhKhanhTour.Helpers
{
    public static class AppState
    {
        // ==========================================
        // QUẢN LÝ NGÔN NGỮ
        // ==========================================
        public static string CurrentLanguageCode { get; private set; } = "vi";
        public static event Action LanguageChanged;

        public static void ChangeLanguage(string newLangCode)
        {
            if (CurrentLanguageCode != newLangCode)
            {
                CurrentLanguageCode = newLangCode;
                LanguageChanged?.Invoke();
            }
        }
    }
}