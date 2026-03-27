using System;

namespace VinhKhanhTour.Helpers
{
    public static class AppState
    {
        // Mặc định ngôn ngữ ban đầu là Tiếng Việt
        public static string CurrentLanguageCode { get; private set; } = "vi";

        // Sự kiện phát loa thông báo
        public static event Action LanguageChanged;

        // Hàm để đổi ngôn ngữ
        public static void ChangeLanguage(string newLangCode)
        {
            if (CurrentLanguageCode != newLangCode)
            {
                CurrentLanguageCode = newLangCode;
                LanguageChanged?.Invoke(); // Kích hoạt sự kiện để các trang khác biết
            }
        }
    }
}