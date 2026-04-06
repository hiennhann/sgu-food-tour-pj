using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Helpers;
using VinhKhanhTour.Data;

namespace VinhKhanhTour.Views
{
    public class LoginPage : ContentPage
    {
        private Entry _emailEntry, _passEntry;

        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            var stack = new VerticalStackLayout { Spacing = 15, Padding = 30, VerticalOptions = LayoutOptions.Center };

            stack.Children.Add(new Label { Text = "Chào mừng trở lại!", FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            stack.Children.Add(new Label { Text = "Đăng nhập để tiếp tục khám phá Quận 4.", FontSize = 15, TextColor = Colors.Gray, Margin = new Thickness(0, 0, 0, 30) });

            _emailEntry = new Entry { Placeholder = "✉️  Email", Keyboard = Keyboard.Email, BackgroundColor = Color.FromArgb("#F4F4F6"), HeightRequest = 55 };
            stack.Children.Add(_emailEntry);

            _passEntry = new Entry { Placeholder = "🔒  Mật khẩu", IsPassword = true, BackgroundColor = Color.FromArgb("#F4F4F6"), HeightRequest = 55 };
            stack.Children.Add(_passEntry);

            var loginBtn = new Button { Text = "ĐĂNG NHẬP", BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 12, HeightRequest = 55, Margin = new Thickness(0, 20, 0, 0) };
            loginBtn.Clicked += OnLoginClicked;
            stack.Children.Add(loginBtn);

            var registerLink = new Label { Text = "Chưa có tài khoản? Đăng ký mới", HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#FF5C0F"), Margin = 15 };
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => await Navigation.PushAsync(new RegisterPage());
            registerLink.GestureRecognizers.Add(tap);
            stack.Children.Add(registerLink);

            Content = stack;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var db = new DatabaseService();
            var user = await db.GetUserAsync(_emailEntry.Text, _passEntry.Text);

            if (user != null)
            {
                // Lưu trạng thái vào AppState để các trang khác biết
                AppState.IsLoggedIn = true;
                AppState.UserName = user.FullName;
                AppState.UserEmail = user.Email;

                await Navigation.PopAsync(); // Quay lại trang Settings
            }
            else
            {
                await DisplayAlert("Thất bại", "Email hoặc mật khẩu không đúng!", "Thử lại");
            }
        }
    }
}