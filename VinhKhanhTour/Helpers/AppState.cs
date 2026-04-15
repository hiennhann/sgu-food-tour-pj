using Microsoft.Maui.Storage;
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
        public static bool IsLoggedIn { get; private set; } = false;

        // Thêm UserId để làm chức năng Nhật ký hành trình
        public static int UserId { get; private set; } = 0;

        public static string UserName { get; private set; } = "";
        public static string UserEmail { get; private set; } = "";

        // Hàm này gọi lúc Đăng Nhập thành công
        public static void LoginSuccess(int id, string name, string email)
        {
            IsLoggedIn = true;
            UserId = id;
            UserName = name;
            UserEmail = email;

            // Lưu vào bộ nhớ điện thoại
            Preferences.Default.Set("IsLoggedIn", true);
            Preferences.Default.Set("UserId", id);
            Preferences.Default.Set("UserName", name);
            Preferences.Default.Set("UserEmail", email);
        }

        // Hàm này gọi lúc Đăng Xuất
        public static void Logout()
        {
            IsLoggedIn = false;
            UserId = 0;
            UserName = "";
            UserEmail = "";

            Preferences.Default.Remove("IsLoggedIn");
            Preferences.Default.Remove("UserId");
            Preferences.Default.Remove("UserName");
            Preferences.Default.Remove("UserEmail");
        }

        // Hàm này gọi ở App.xaml.cs khi App vừa khởi động
        public static void LoadSavedSession()
        {
            IsLoggedIn = Preferences.Default.Get("IsLoggedIn", false);
            if (IsLoggedIn)
            {
                UserId = Preferences.Default.Get("UserId", 0);
                UserName = Preferences.Default.Get("UserName", "");
                UserEmail = Preferences.Default.Get("UserEmail", "");
            }
        }
    }
}