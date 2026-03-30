using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using VinhKhanhTour.Services;

namespace VinhKhanhTour.Helpers
{
    // Class này sẽ đứng ra làm trung gian dịch thuật, tránh việc MAUI tự động parse chuỗi
    public class TranslateConverter : IValueConverter
    {
        // Khởi tạo một phiên bản duy nhất để dùng chung+
        public static TranslateConverter Instance { get; } = new TranslateConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string key)
            {
                // Lấy từ khóa từ parameter và gọi từ điển bằng C# thuần (không bao giờ lỗi)
                return LocalizationResourceManager.Instance[key];
            }
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}