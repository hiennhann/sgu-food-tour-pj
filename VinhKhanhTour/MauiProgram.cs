using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting; // BẮT BUỘC cho Map
using Microsoft.Maui.Hosting;
using System;
using VinhKhanhTour.Data;

namespace VinhKhanhTour
{
    public static class MauiProgram
    {
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

            // Đăng ký Database
            builder.Services.AddSingleton<PoiRepository>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}