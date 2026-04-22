using Microsoft.AspNetCore.SignalR.Client;
using VinhKhanhTour.Services;
using VinhKhanhTour.Views;
using System;

namespace VinhKhanhTour
{
    public partial class App : Application
    {
        // Dùng SharedHub public static để các trang khác có thể gọi (gửi tọa độ, nghe heatmap...)
        public static HubConnection SharedHub { get; private set; }
        public static event EventHandler<string> HeatmapDataReceived;

        private readonly SubscriptionService _subscriptionService;

        public App()
        {
            InitializeComponent();

            _subscriptionService = new SubscriptionService();

            // 1. Gọi hàm setup SignalR
            SetupSignalR();
        }

        private void SetupSignalR()
        {
            string deviceId = Preferences.Default.Get("UniqueDeviceId", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString().Substring(0, 8);
                Preferences.Default.Set("UniqueDeviceId", deviceId);
            }

            // Dùng link Dev Tunnels cố định
            string hubUrl = $"https://9x12w3qg-5113.asse.devtunnels.ms/monitoringHub?deviceId={deviceId}";

            SharedHub = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    // Quan trọng: Ép bỏ qua trang cảnh báo của Microsoft
                    options.Headers.Add("X-Tunnel-Skip-AntiPhishing-Page", "true");
                })
                .WithAutomaticReconnect()
                .Build();

            // Lắng nghe sự kiện cập nhật bản đồ nhiệt
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
            // Trạng thái ban đầu: Nạp HomePage
            return new Window(new NavigationPage(new HomePage()));
        }

        // 2. Kích hoạt kết nối và KIỂM TRA BẢN QUYỀN khi app bắt đầu chạy
        protected override async void OnStart()
        {
            base.OnStart();

            // --- PHẦN A: BẬT SIGNALR (Kèm bắt lỗi hiện popup) ---
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

            // --- PHẦN B: KIỂM TRA BẢN QUYỀN TRẢ PHÍ ---
            string deviceId = Preferences.Default.Get("UniqueDeviceId", string.Empty);
            var status = await _subscriptionService.CheckStatusAsync(deviceId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (status != null && status.Status == "Expired")
                {
                    // Lập tức cắt đứt NavigationPage, đá văng về trang thanh toán
                    Application.Current.MainPage = new PaywallPage();
                }
                else if (status != null && status.Status == "Trial")
                {
                    // Code hiện thông báo dùng thử (nếu có)
                }
            });
        }

        protected override async void OnSleep()
        {
            base.OnSleep();
            try { await SharedHub.StopAsync(); } catch { }
        }

        // --- PHẦN C: HỨNG LINK QUÉT MÃ QR (DEEP LINKING) ---
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // Kiểm tra xem khách có vừa quét mã vinhkhanhtour.com/unlock không
            if (uri.Host == "vinhkhanhtour.com" && uri.AbsolutePath == "/unlock")
            {
                // Lưu vào bộ nhớ là đã kích hoạt
                Preferences.Default.Set("IsAppUnlocked", true);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Current.MainPage.DisplayAlert("Kích hoạt thành công", "Mời bạn bắt đầu chuyến Food Tour Vĩnh Khánh!", "Tuyệt vời");

                    // Xóa màn hình chờ, nạp lại HomePage
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                });
            }
        }
    }
}