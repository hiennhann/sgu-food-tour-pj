using Microsoft.AspNetCore.SignalR.Client;
using VinhKhanhTour.Services;
using VinhKhanhTour.Views;
using System;

namespace VinhKhanhTour
{
    public partial class App : Application
    {
        public static HubConnection SharedHub { get; private set; }
        public static event EventHandler<string> HeatmapDataReceived;

        private readonly SubscriptionService _subscriptionService;

        public App()
        {
            InitializeComponent();
            _subscriptionService = new SubscriptionService();
            SetupSignalR();

            // 1. NGƯỜI GÁC CỔNG: Kiểm tra xem đã quét mã QR chưa
            bool isUnlocked = Preferences.Default.Get("IsAppUnlocked", false);

            if (isUnlocked)
            {
                // Đã quét -> Vào thẳng trang chủ
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                // Chưa quét -> Khóa ngoài cửa
                MainPage = CreateLockPage();
            }
        }

        private void SetupSignalR()
        {
            string deviceId = Preferences.Default.Get("UniqueDeviceId", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString().Substring(0, 8);
                Preferences.Default.Set("UniqueDeviceId", deviceId);
            }

            string hubUrl = $"https://9x12w3qg-5113.asse.devtunnels.ms/monitoringHub?deviceId={deviceId}";

            SharedHub = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.Headers.Add("X-Tunnel-Skip-AntiPhishing-Page", "true");
                })
                .WithAutomaticReconnect()
                .Build();

            SharedHub.On<string>("UpdateHeatmap", (jsonStr) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HeatmapDataReceived?.Invoke(null, jsonStr);
                });
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Trả về MainPage đã được quyết định ở hàm App() phía trên
            return new Window(MainPage);
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Bật SignalR
            try
            {
                await SharedHub.StartAsync();
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Lỗi SignalR", ex.Message, "OK");
                });
            }

            // 2. CHỈ KIỂM TRA TRẢ PHÍ (PAYWALL) NẾU APP ĐÃ ĐƯỢC MỞ KHÓA BẰNG QR
            bool isUnlocked = Preferences.Default.Get("IsAppUnlocked", false);
            if (isUnlocked)
            {
                string deviceId = Preferences.Default.Get("UniqueDeviceId", string.Empty);
                var status = await _subscriptionService.CheckStatusAsync(deviceId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (status != null && status.Status == "Expired")
                    {
                        // Hết hạn thì đá qua trang thanh toán (Nhớ truyền tham số khóa màn hình nếu có)
                        Application.Current.MainPage = new PaywallPage();
                    }
                });
            }
        }

        protected override async void OnSleep()
        {
            base.OnSleep();
            try { await SharedHub.StopAsync(); } catch { }
        }

        // 3. CHIẾC CHÌA KHÓA: HỨNG LINK TỪ CAMERA MẶC ĐỊNH
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // Dùng ToLower() để an toàn tuyệt đối khi so sánh chuỗi
            if (uri.Host.ToLower() == "vkt" && uri.AbsolutePath.ToLower() == "/unlock")
            {
                // Lưu vào bộ nhớ là đã kích hoạt vĩnh viễn
                Preferences.Default.Set("IsAppUnlocked", true);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Current.MainPage.DisplayAlert("Thành công", "Vé của bạn đã được xác nhận. Chào mừng đến với Vĩnh Khánh Tour!", "Bắt đầu ngay");

                    // Phá khóa màn hình, đẩy khách vào trang chủ
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                });
            }
        }

        // 4. GIAO DIỆN MÀN HÌNH KHÓA (Code UI trực tiếp, không cần tạo file XAML mới)
        private ContentPage CreateLockPage()
        {
            return new ContentPage
            {
                BackgroundColor = Color.FromArgb("#F5F5F5"),
                Content = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Padding = 30,
                    Children = {
                        new Label {
                            Text = "🔒 CHƯA KÍCH HOẠT TOUR",
                            FontSize = 24, FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center, TextColor = Colors.Red
                        },
                        new Label {
                            Text = "Vui lòng thoát App và dùng Camera điện thoại quét mã QR trên vé Tour của bạn để bắt đầu trải nghiệm.",
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontSize = 16, TextColor = Colors.Gray
                        }
                    }
                }
            };
        }
    }
}