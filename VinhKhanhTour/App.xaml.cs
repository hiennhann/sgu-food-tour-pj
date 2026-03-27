// Nếu bạn để MainPage.cs trong thư mục Views thì thêm dòng này:
// using VinhKhanhTour.Views; 

namespace VinhKhanhTour // (Hoặc VinhKhanhFoodTour tùy tên project hiện tại của bạn)
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Bọc MainPage trong NavigationPage để sau này dễ chuyển qua trang Audio
            return new Window(new NavigationPage(new HomePage()));
        }
    }
}