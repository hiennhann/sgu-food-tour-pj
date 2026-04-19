using Microsoft.Extensions.Logging;
using VinhKhanhTour.Services;
using VinhKhanhTour.Data; // Thêm dòng này

namespace VinhKhanhTour
{
    public static class MauiProgram
    {
        // Khai báo một biến tĩnh để các file khác có thể gọi được Services
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ===============================================
            // ĐĂNG KÝ TẤT CẢ SERVICES VÀ REPOSITORY Ở ĐÂY
            // ===============================================
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddSingleton<PoiRepository>();   // ĐÂY LÀ THẰNG CÒN THIẾU GÂY LỖI NULL!
                                                              // Đăng ký các Service để App biết đường gọi
            builder.Services.AddSingleton<VinhKhanhTour.Services.ApiService>();
            builder.Services.AddSingleton<VinhKhanhTour.Data.PoiRepository>(); // DÒNG NÀY RẤT QUAN TRỌNG
            var app = builder.Build();

            // Gán Services để dùng toàn cục
            Services = app.Services;

            return app;
        }
    }
}