using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using VinhKhanhTour.Helpers;
using VinhKhanhTour.Services;
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

            // Dùng Grid để chia màn hình làm 2 phần: Nút Back ở trên, Form ở dưới
            var mainGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // Chứa nút Back
                    new RowDefinition { Height = GridLength.Star }  // Chứa Form
                }
            };

            // ==========================================
            // NÚT BACK (QUAY LẠI)
            // ==========================================
            var backLayout = new HorizontalStackLayout { Padding = new Thickness(20, 20, 0, 0), Spacing = 5 };
            var backIcon = new Label { Text = "←", FontSize = 24, TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center };
            var backText = new Label { TextColor = Colors.Black, FontSize = 16, VerticalOptions = LayoutOptions.Center };
            backText.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Quay lại"));

            backLayout.Children.Add(backIcon);
            backLayout.Children.Add(backText);

            var tapBack = new TapGestureRecognizer();
            tapBack.Tapped += async (s, e) => await Navigation.PopAsync();
            backLayout.GestureRecognizers.Add(tapBack);

            Grid.SetRow(backLayout, 0);
            mainGrid.Children.Add(backLayout);

            // ==========================================
            // FORM ĐĂNG NHẬP
            // ==========================================
            var stack = new VerticalStackLayout { Spacing = 15, Padding = 30, VerticalOptions = LayoutOptions.Center };

            var titleLabel = new Label { FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Chào mừng trở lại!"));
            stack.Children.Add(titleLabel);

            var subtitleLabel = new Label { FontSize = 15, TextColor = Colors.Gray, Margin = new Thickness(0, 0, 0, 30) };
            subtitleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Đăng nhập để tiếp tục khám phá Quận 4."));
            stack.Children.Add(subtitleLabel);

            _emailEntry = new Entry { Keyboard = Keyboard.Email, BackgroundColor = Color.FromArgb("#F4F4F6"), HeightRequest = 55 };
            _emailEntry.SetBinding(Entry.PlaceholderProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Email", stringFormat: "✉️  {0}"));
            stack.Children.Add(_emailEntry);

            _passEntry = new Entry { IsPassword = true, BackgroundColor = Color.FromArgb("#F4F4F6"), HeightRequest = 55 };
            _passEntry.SetBinding(Entry.PlaceholderProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Mật khẩu", stringFormat: "🔒  {0}"));
            stack.Children.Add(_passEntry);

            var loginBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 12, HeightRequest = 55, Margin = new Thickness(0, 20, 0, 0) };
            loginBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "ĐĂNG NHẬP"));
            loginBtn.Clicked += OnLoginClicked;
            stack.Children.Add(loginBtn);

            var registerLink = new Label { HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#FF5C0F"), Margin = 15 };
            registerLink.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Chưa có tài khoản? Đăng ký mới"));
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => await Navigation.PushAsync(new RegisterPage());
            registerLink.GestureRecognizers.Add(tap);
            stack.Children.Add(registerLink);

            Grid.SetRow(stack, 1);
            mainGrid.Children.Add(stack);

            Content = mainGrid;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var db = new DatabaseService();
            var user = await db.GetUserAsync(_emailEntry.Text, _passEntry.Text);

            if (user != null)
            {
                AppState.LoginSuccess(user.Id, user.FullName, user.Email);
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Thất bại", "Email hoặc mật khẩu không đúng!", "Thử lại");
            }
        }
    }
}