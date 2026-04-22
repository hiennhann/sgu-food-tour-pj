using VinhKhanhTour.Services;

namespace VinhKhanhTour.Views
{
    public partial class PaywallPage : ContentPage
    {
        private string _deviceId;
        private readonly SubscriptionService _subscriptionService;
        private bool _isLockedMode;

        // Thêm tham số cờ isLockedMode để biết màn hình này là tự nguyện bật hay bị ép bật
        public PaywallPage(bool isLockedMode = true)
        {
            InitializeComponent();
            _subscriptionService = new SubscriptionService();
            _isLockedMode = isLockedMode;

            if (isLockedMode)
            {
                // Bị ép bật (Hết hạn 24h) -> Khóa phím Back
                Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            }
            else
            {
                // Tự nguyện bật từ Setting (Chạy bình thường) -> Đổi chữ nhẹ nhàng
                TitleLabel.Text = "NÂNG CẤP PREMIUM";
                TitleLabel.TextColor = Color.FromArgb("#FF5C0F");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _deviceId = Preferences.Default.Get("UniqueDeviceId", string.Empty);

            if (!string.IsNullOrEmpty(_deviceId))
            {
                TransferContentLabel.Text = $"Nội dung CK: VKT {_deviceId}";

                // Mã BIN của ngân hàng BIDV là 970418
                string bankBin = "970418";
                string bankAccount = "1890609952";
                string accName = "ADMIN APP"; // Đổi thành tên chủ tài khoản thật nếu muốn
                int amount = 49000;
                string content = $"VKT {_deviceId}";

                string vietQrUrl = $"https://img.vietqr.io/image/{bankBin}-{bankAccount}-compact2.png?amount={amount}&addInfo={content}&accountName={accName}";

                QRCodeImage.Source = ImageSource.FromUri(new Uri(vietQrUrl));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Trả về true nghĩa là "Tôi đã xử lý nút back rồi, hệ thống đừng làm gì nữa (tức là chặn lại)"
            // Trả về false nghĩa là "Cứ lùi trang thoải mái"
            return _isLockedMode;
        }

        private async void OnCheckPaymentClicked(object sender, EventArgs e)
        {
            var status = await _subscriptionService.CheckStatusAsync(_deviceId);

            if (status != null && status.Status == "Premium")
            {
                await DisplayAlert("Thành công", "Cảm ơn bạn đã thanh toán. Chúc bạn trải nghiệm App vui vẻ!", "OK");

                if (_isLockedMode)
                {
                    // Từ luồng khóa màn văng ra
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    // Từ luồng Setting mở vào, lùi về Settings
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("Chưa nhận được phí", "Hệ thống vẫn chưa ghi nhận thanh toán cho thiết bị này. Vui lòng thử lại sau ít phút hoặc liên hệ Admin.", "Đóng");
            }
        }
    }
}