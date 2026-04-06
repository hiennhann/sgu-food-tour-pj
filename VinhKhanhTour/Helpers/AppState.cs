using System;

namespace VinhKhanhTour.Helpers
{
    public static class AppState
    {
        // ==========================================
        // 1. QUẢN LÝ NGÔN NGỮ
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

        // ==========================================
        // 2. QUẢN LÝ TRẠNG THÁI ĐĂNG NHẬP
        // ==========================================
        public static bool IsLoggedIn { get; set; } = false;
        public static string UserName { get; set; } = "";
        public static string UserEmail { get; set; } = "";

        public static void Logout()
        {
            IsLoggedIn = false;
            UserName = "";
            UserEmail = "";
        }
    }
}