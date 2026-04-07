using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Models;
using VinhKhanhTour.Data;
using VinhKhanhTour.Services;
using VinhKhanhTour.Helpers;

namespace VinhKhanhTour.Views
{
    public class RegisterPage : ContentPage
    {
        private Entry _nameEntry, _emailEntry, _passEntry;

        public RegisterPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            var stack = new VerticalStackLayout { Spacing = 15, Padding = 30, VerticalOptions = LayoutOptions.Center };

            var titleLabel = new Label { FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Tạo tài khoản mới"));
            stack.Children.Add(titleLabel);

            var subtitleLabel = new Label { FontSize = 15, TextColor = Colors.Gray, Margin = new Thickness(0, 0, 0, 20) };
            subtitleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Tham gia cộng đồng ẩm thực Vĩnh Khánh ngay."));
            stack.Children.Add(subtitleLabel);

            _nameEntry = CreateStyledEntry("Họ và tên", "👤");
            stack.Children.Add(_nameEntry);

            _emailEntry = CreateStyledEntry("Email", "✉️", Keyboard.Email);
            stack.Children.Add(_emailEntry);

            _passEntry = CreateStyledEntry("Mật khẩu", "🔒", isPassword: true);
            stack.Children.Add(_passEntry);

            var regBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 12, HeightRequest = 55, Margin = new Thickness(0, 20, 0, 0) };
            regBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "ĐĂNG KÝ NGAY"));
            regBtn.Clicked += OnRegisterClicked;
            stack.Children.Add(regBtn);

            var loginLink = new Label { HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#FF5C0F"), Margin = 10 };
            loginLink.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Đã có tài khoản? Đăng nhập"));
            var tap = new TapGestureRecognizer(); tap.Tapped += async (s, e) => await Navigation.PopAsync();
            loginLink.GestureRecognizers.Add(tap);
            stack.Children.Add(loginLink);

            Content = new ScrollView { Content = stack };
        }

        private Entry CreateStyledEntry(string placeholderKey, string icon, Keyboard k = null, bool isPassword = false)
        {
            var entry = new Entry { IsPassword = isPassword, Keyboard = k ?? Keyboard.Default, BackgroundColor = Color.FromArgb("#F4F4F6"), HeightRequest = 55, Margin = new Thickness(0, 5) };
            entry.SetBinding(Entry.PlaceholderProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: placeholderKey, stringFormat: $"{icon}  {{0}}"));
            return entry;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_emailEntry.Text) || string.IsNullOrWhiteSpace(_passEntry.Text))
            {
                await DisplayAlert("Lỗi", "Vui lòng điền đủ thông tin", "OK"); return;
            }

            var db = new DatabaseService();
            if (await db.IsEmailExistsAsync(_emailEntry.Text))
            {
                await DisplayAlert("Lỗi", "Email này đã được đăng ký", "OK"); return;
            }

            await db.RegisterUserAsync(new UserAccount { FullName = _nameEntry.Text, Email = _emailEntry.Text, Password = _passEntry.Text });
            await DisplayAlert("Thành công", "Tài khoản đã sẵn sàng!", "Đăng nhập ngay");
            await Navigation.PopAsync();
        }
    }
}