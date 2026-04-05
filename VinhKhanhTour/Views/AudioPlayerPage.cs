using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using System;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;

namespace VinhKhanhTour.Views
{
    public class AudioPlayerPage : ContentPage
    {
        private FoodPlace _currentPlace;
        private bool _isPlaying = false;
        private Button _playPauseBtn;

        public AudioPlayerPage(FoodPlace place)
        {
            _currentPlace = place;
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            var mainStack = new VerticalStackLayout { Spacing = 20 };

            var headerGrid = new Grid { BackgroundColor = Color.FromArgb("#FF5C0F"), HeightRequest = 60, Padding = new Thickness(15, 0) };

            // SỬA LẠI: Dùng nút chữ (Button) thay vì nút ảnh (ImageButton) để chống tàng hình
            var backBtn = new Button
            {
                Text = "←",
                FontSize = 28,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(0),
                WidthRequest = 50,
                HeightRequest = 50
            };
            backBtn.Clicked += async (s, e) => {
                NarrationEngine.Instance.CancelCurrentNarration(); // Tắt audio khi thoát
                await Navigation.PopAsync();
            };
            headerGrid.Children.Add(backBtn);

            var titleLabel = new Label { Text = place.Name, TextColor = Colors.White, FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            headerGrid.Children.Add(titleLabel);
            mainStack.Children.Add(headerGrid);
            // 2. TRÌNH PHÁT AUDIO (Thẻ màu hồng nhạt)
            var playerCard = new Border
            {
                BackgroundColor = Color.FromArgb("#FFF5F2"),
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                Margin = new Thickness(20, 20, 20, 0),
                Padding = new Thickness(20, 30)
            };

            var playerStack = new VerticalStackLayout { Spacing = 25 };

            // Thanh trượt (Slider)
            var slider = new Slider { MinimumTrackColor = Color.FromArgb("#FF5C0F"), MaximumTrackColor = Color.FromArgb("#FFDAB9"), ThumbColor = Color.FromArgb("#FF5C0F") };
            playerStack.Children.Add(slider);

            // CÁC NÚT ĐIỀU KHIỂN (Đã được thêm vào)
            var controlLayout = new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Center, Spacing = 30, VerticalOptions = LayoutOptions.Center };

            // Nút Stop (Dừng)
            var stopBtn = new Button { Text = "⏹", FontSize = 24, BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#FF5C0F"), WidthRequest = 50, HeightRequest = 50 };
            stopBtn.Clicked += (s, e) => {
                _isPlaying = false;
                _playPauseBtn.Text = "▶";
                NarrationEngine.Instance.CancelCurrentNarration();
            };

            // Nút Play / Pause
            _playPauseBtn = new Button { Text = "▶", FontSize = 32, BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, WidthRequest = 64, HeightRequest = 64, CornerRadius = 32, Padding = 0 };
            _playPauseBtn.Clicked += async (s, e) => {
                if (_isPlaying)
                {
                    _isPlaying = false;
                    _playPauseBtn.Text = "▶";
                    NarrationEngine.Instance.CancelCurrentNarration();
                }
                else
                {
                    _isPlaying = true;
                    _playPauseBtn.Text = "⏸";
                    await NarrationEngine.Instance.PlayNarrationAsync(_currentPlace, isManual: true);

                    // Reset lại nút Play khi đọc xong
                    _isPlaying = false;
                    _playPauseBtn.Text = "▶";
                }
            };

            // Nút Next (Giả lập)
            var nextBtn = new Button { Text = "⏭", FontSize = 24, BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#FF5C0F"), WidthRequest = 50, HeightRequest = 50 };

            controlLayout.Children.Add(stopBtn);
            controlLayout.Children.Add(_playPauseBtn);
            controlLayout.Children.Add(nextBtn);

            playerStack.Children.Add(controlLayout);
            playerCard.Content = playerStack;
            mainStack.Children.Add(playerCard);

            // 3. THÔNG TIN QUÁN
            var contentStack = new VerticalStackLayout { Padding = new Thickness(20, 0), Spacing = 10 };

            var infoTitle = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            infoTitle.SetBinding(Label.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: $"Giới thiệu về {_currentPlace.Name}"
            ));
            contentStack.Children.Add(infoTitle);

            var descLabel = new Label { Text = _currentPlace.NarrationText, FontSize = 14, TextColor = Colors.Gray, LineHeight = 1.4 };
            contentStack.Children.Add(descLabel);
            mainStack.Children.Add(contentStack);

            // 4. HAI NÚT CHỨC NĂNG (Chỉ đường & Thực đơn)
            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15, Padding = new Thickness(20) };

            var routeBtn = new Button { BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 15, HeightRequest = 50 };
            routeBtn.SetBinding(Button.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: "Chỉ Đường",
                stringFormat: "📍 {0}"
            ));
            routeBtn.Clicked += async (s, e) => {
                var options = new MapLaunchOptions { Name = _currentPlace.Name, NavigationMode = NavigationMode.Driving };
                // Truyền tọa độ gốc của quán (Giả lập tạm thời, nếu bạn có truyền kinh/vĩ độ qua FoodPlace thì thay vào đây)
                var location = new Location(10.761622, 106.661172);
                try { await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options); }
                catch { await DisplayAlert("Lỗi", "Không thể mở ứng dụng bản đồ.", "OK"); }
            };
            Grid.SetColumn(routeBtn, 0);
            actionGrid.Children.Add(routeBtn);

            var menuBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 15, HeightRequest = 50 };
            menuBtn.SetBinding(Button.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: "Xem Thực Đơn",
                stringFormat: "📋 {0}"
            ));
            menuBtn.Clicked += async (s, e) => await Navigation.PushAsync(new MenuPage(_currentPlace));
            Grid.SetColumn(menuBtn, 1);
            actionGrid.Children.Add(menuBtn);

            mainStack.Children.Add(actionGrid);

            Content = new ScrollView { Content = mainStack };
        }
    }
}