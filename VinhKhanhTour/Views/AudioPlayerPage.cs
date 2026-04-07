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
            BindingContext = _currentPlace; // Gắn context để load ảnh
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            // Dùng Grid chia màn hình: Phần trên là nội dung, Phần dưới là Thanh nút bấm cố định
            var rootGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star }, // Vùng cuộn
                    new RowDefinition { Height = GridLength.Auto }  // Thanh công cụ đáy
                }
            };

            var scrollContent = new ScrollView();
            var mainStack = new VerticalStackLayout { Spacing = 15 };

            // 1. THANH TOP BAR & NÚT BACK (Viên thuốc)
            var topBar = new HorizontalStackLayout
            {
                Padding = new Thickness(20, 15, 20, 5)
            };

            var backBtn = new Button
            {
                Text = "Back",
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#FF5C0F"),
                BackgroundColor = Color.FromArgb("#FFF0ED"),
                HeightRequest = 40,
                CornerRadius = 20,
                Padding = new Thickness(20, 0),
                HorizontalOptions = LayoutOptions.Start
            };
            backBtn.Clicked += async (s, e) => {
                NarrationEngine.Instance.CancelCurrentNarration(); // Tắt audio khi thoát
                await Navigation.PopAsync();
            };
            topBar.Children.Add(backBtn);
            mainStack.Children.Add(topBar);

            // 2. ẢNH BÌA (ALBUM ART)
            var imageBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 30 },
                HeightRequest = 300,
                Margin = new Thickness(30, 0, 30, 10),
                Stroke = Colors.Transparent,
                Shadow = new Shadow { Brush = Color.FromArgb("#FF5C0F"), Opacity = 0.2f, Offset = new Point(0, 10), Radius = 20 }
            };
            var coverImage = new Image { Aspect = Aspect.AspectFill };
            coverImage.SetBinding(Image.SourceProperty, "ImageUrl");
            imageBorder.Content = coverImage;
            mainStack.Children.Add(imageBorder);

            // 3. TIÊU ĐỀ QUÁN
            var titleStack = new VerticalStackLayout { Spacing = 5, Margin = new Thickness(30, 0) };
            var titleLabel = new Label { Text = _currentPlace.Name, FontSize = 26, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
            var metaLabel = new Label { Text = "🎧 Thuyết minh tự động", FontSize = 14, TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };

            titleStack.Children.Add(titleLabel);
            titleStack.Children.Add(metaLabel);
            mainStack.Children.Add(titleStack);

            // 4. TRÌNH PHÁT AUDIO (Đã bổ sung chuẩn Try...Finally)
            var playerStack = new VerticalStackLayout { Margin = new Thickness(30, 15, 30, 10) };

            _playPauseBtn = new Button
            {
                BackgroundColor = Color.FromArgb("#FF5C0F"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                CornerRadius = 25,
                HeightRequest = 55,
                Shadow = new Shadow { Brush = Color.FromArgb("#FF5C0F"), Opacity = 0.4f, Offset = new Point(0, 5), Radius = 10 }
            };
            _playPauseBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "▶️ {0}"));

            _playPauseBtn.Clicked += async (s, e) => {
                if (_isPlaying)
                {
                    // Đang phát -> Bấm để Dừng
                    _isPlaying = false;
                    _playPauseBtn.BackgroundColor = Color.FromArgb("#FF5C0F");
                    _playPauseBtn.Shadow.Brush = Color.FromArgb("#FF5C0F");
                    _playPauseBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "▶️ {0}"));
                    NarrationEngine.Instance.CancelCurrentNarration();
                }
                else
                {
                    // Bấm để Phát -> Chuyển nút sang màu đỏ (Trạng thái Dừng)
                    _isPlaying = true;
                    _playPauseBtn.BackgroundColor = Color.FromArgb("#E53935");
                    _playPauseBtn.Shadow.Brush = Color.FromArgb("#E53935");
                    _playPauseBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Dừng Audio", stringFormat: "⏹ {0}"));

                    try
                    {
                        // Chờ đọc xong Audio
                        await NarrationEngine.Instance.PlayNarrationAsync(_currentPlace, isManual: true);
                    }
                    finally
                    {
                        // FINALLY: Tự động nhả nút về màu cam khi kết thúc (hoặc lỗi)
                        if (_isPlaying)
                        {
                            _isPlaying = false;
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                _playPauseBtn.BackgroundColor = Color.FromArgb("#FF5C0F");
                                _playPauseBtn.Shadow.Brush = Color.FromArgb("#FF5C0F");
                                _playPauseBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "▶️ {0}"));
                            });
                        }
                    }
                }
            };

            playerStack.Children.Add(_playPauseBtn);
            mainStack.Children.Add(playerStack);

            // 5. THÔNG TIN QUÁN (Văn bản thuyết minh)
            var contentStack = new VerticalStackLayout { Padding = new Thickness(30, 10, 30, 40), Spacing = 10 };

            var infoTitle = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            infoTitle.SetBinding(Label.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: $"Nội dung thuyết minh"
            ));
            contentStack.Children.Add(infoTitle);

            var descLabel = new Label { Text = _currentPlace.NarrationText, FontSize = 15, TextColor = Color.FromArgb("#505050"), LineHeight = 1.6 };
            contentStack.Children.Add(descLabel);
            mainStack.Children.Add(contentStack);

            scrollContent.Content = mainStack;
            Grid.SetRow(scrollContent, 0);
            rootGrid.Children.Add(scrollContent);

            // ==========================================
            // PHẦN 6: THANH CÔNG CỤ ĐÁY CỐ ĐỊNH 
            // ==========================================
            var bottomBar = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Padding = new Thickness(20, 15, 20, 25),
                Shadow = new Shadow { Brush = Colors.Black, Radius = 10, Opacity = 0.08f, Offset = new Point(0, -4) }
            };

            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };

            var routeBtn = new Button { BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 55 };
            routeBtn.SetBinding(Button.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: "Chỉ Đường",
                stringFormat: "📍 {0}"
            ));
            routeBtn.Clicked += async (s, e) => {
                var options = new MapLaunchOptions { Name = _currentPlace.Name, NavigationMode = NavigationMode.Driving };
                var location = new Location(10.761622, 106.661172); // Tọa độ mẫu
                try { await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options); }
                catch { await DisplayAlert("Lỗi", "Không thể mở ứng dụng bản đồ.", "OK"); }
            };
            Grid.SetColumn(routeBtn, 0);
            actionGrid.Children.Add(routeBtn);

            var menuBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 55 };
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

            bottomBar.Content = actionGrid;
            Grid.SetRow(bottomBar, 1);
            rootGrid.Children.Add(bottomBar);

            Content = rootGrid;
        }
    }
}