using Microsoft.AspNetCore.SignalR.Client;

namespace VinhKhanhTour
{
    public partial class App : Application
    {
        private HubConnection _hubConnection;

        public App()
        {
            InitializeComponent();

            // 1. Gọi hàm setup SignalR ở đây
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

            // Nhớ thay IP đúng của máy bạn hoặc link Dev Tunnels
            string hubUrl = $"http://10.0.2.2:5113/monitoringHub?deviceId={deviceId}";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Vẫn giữ logic NavigationPage của bạn
            return new Window(new NavigationPage(new HomePage()));
        }

        // 2. Kích hoạt kết nối khi app bắt đầu chạy
        protected override async void OnStart()
        {
            base.OnStart();
            try { await _hubConnection.StartAsync(); } catch { }
        }

        protected override async void OnSleep()
        {
            base.OnSleep();
            try { await _hubConnection.StopAsync(); } catch { }
        }
    }
}