using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Media;
using System;
using System.Linq;
using System.Threading;
using VinhKhanhTour.Helpers;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;

namespace VinhKhanhTour.Views
{
    public class AudioPlayerPage : ContentPage
    {
        private FoodPlace _currentPlace;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isPlaying = false;
        private ImageButton _playPauseBtn;

        public AudioPlayerPage(FoodPlace place)
        {
            _currentPlace = place;
            BindingContext = place; // Ràng buộc dữ liệu vào trang

            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#5D1E0F");

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 80 }
                }
            };

            // --- HÀNG 1: HEADER ---
            var headerGrid = new Grid
            {
                BackgroundColor = Color.FromArgb("#E68A00"),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 50 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 50 }
                }
            };

            var backBtn = new ImageButton { Source = "icon_back_white.png", WidthRequest = 24, HeightRequest = 24, HorizontalOptions = LayoutOptions.Center };
            backBtn.Clicked += async (s, e) =>
            {
                StopAudio(); // Dừng đọc khi bấm Back thoát trang
                await Navigation.PopAsync();
            };
            Grid.SetColumn(backBtn, 0);
            headerGrid.Children.Add(backBtn);

            // BINDING Tiêu đề
            var titleLabel = new Label { TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 18, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            titleLabel.SetBinding(Label.TextProperty, "Name");
            Grid.SetColumn(titleLabel, 1);
            headerGrid.Children.Add(titleLabel);

            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);

            // --- HÀNG 2: NỘI DUNG CHÍNH ---
            var contentScrollView = new ScrollView();
            var contentStack = new VerticalStackLayout { Padding = new Thickness(15), Spacing = 20 };

            // 1. Ảnh món ăn
            var imageGrid = new Grid { HeightRequest = 220 };
            var mainImage = new Image { Aspect = Aspect.AspectFill };
            mainImage.SetBinding(Image.SourceProperty, "ImageUrl");

            imageGrid.Children.Add(new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                Stroke = Colors.Transparent,
                Content = mainImage
            });
            contentStack.Children.Add(imageGrid);

            // 2. Khu vực Audio Player
            var playerBorder = new Border
            {
                BackgroundColor = Color.FromArgb("#4A1508"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                Stroke = Colors.Transparent,
                Padding = new Thickness(15, 20)
            };

            var playerStack = new VerticalStackLayout { Spacing = 15 };

            // Cụm nút bấm
            var controlButtonsStack = new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Center, Spacing = 30, VerticalOptions = LayoutOptions.Center };

            _playPauseBtn = new ImageButton { Source = "icon_play_orange_circle.png", WidthRequest = 60, HeightRequest = 60 };
            _playPauseBtn.Clicked += OnPlayPauseButtonClicked; // Gắn sự kiện đọc Audio

            controlButtonsStack.Children.Add(new ImageButton { Source = "icon_rewind_10s.png", WidthRequest = 35, HeightRequest = 35 });
            controlButtonsStack.Children.Add(_playPauseBtn);
            controlButtonsStack.Children.Add(new ImageButton { Source = "icon_forward_10s.png", WidthRequest = 35, HeightRequest = 35 });
            playerStack.Children.Add(controlButtonsStack);

            // Thanh tiến trình (ĐÃ FIX LỖI)
            var timeGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 40 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 40 } }, ColumnSpacing = 10 };
            var timeSlider = new Slider { Minimum = 0, Maximum = 100, Value = 0, MinimumTrackColor = Color.FromArgb("#E68A00"), ThumbColor = Colors.White };
            Grid.SetColumn(timeSlider, 1);
            timeGrid.Children.Add(timeSlider);
            playerStack.Children.Add(timeGrid);

            playerBorder.Content = playerStack;
            contentStack.Children.Add(playerBorder);

            // 3. Text Mô tả
            var aboutTitle = new Label { TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 18 };
            string formatString = LocalizationResourceManager.Instance["Giới thiệu về {0}"] ?? "Giới thiệu về {0}";
            aboutTitle.SetBinding(Label.TextProperty, new Binding("Name", stringFormat: formatString));
            contentStack.Children.Add(aboutTitle);

            var descLabel = new Label { TextColor = Color.FromArgb("#F0D6D0"), FontSize = 14, LineHeight = 1.4 };
            descLabel.SetBinding(Label.TextProperty, "NarrationText");
            contentStack.Children.Add(descLabel);

            // 4. CÁC NÚT HÀNH ĐỘNG
            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15, Margin = new Thickness(0, 10, 0, 20) };

            var routeBtn = new Button { BackgroundColor = Color.FromArgb("#D9801A"), TextColor = Colors.White, CornerRadius = 25, HeightRequest = 50 };
            routeBtn.SetBinding(Button.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: TranslateConverter.Instance,
                converterParameter: "Chỉ Đường",
                stringFormat: "📍 {0}"
            ));
            Grid.SetColumn(routeBtn, 0);
            actionGrid.Children.Add(routeBtn);

            var menuBtn = new Button { BackgroundColor = Color.FromArgb("#7B4335"), TextColor = Colors.White, BorderColor = Color.FromArgb("#D9801A"), BorderWidth = 1, CornerRadius = 25, HeightRequest = 50 };
            menuBtn.SetBinding(Button.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: TranslateConverter.Instance,
                converterParameter: "Xem Thực Đơn",
                stringFormat: "📋 {0}"
            ));
            menuBtn.Clicked += async (s, e) => await Navigation.PushAsync(new MenuPage(place));
            Grid.SetColumn(menuBtn, 1);
            actionGrid.Children.Add(menuBtn);

            contentStack.Children.Add(actionGrid);

            // --- KẾT THÚC HÀNG 2 ---
            contentScrollView.Content = contentStack;
            Grid.SetRow(contentScrollView, 1);
            mainGrid.Children.Add(contentScrollView);

            Content = mainGrid;
        }

        // HÀM XỬ LÝ PLAY/PAUSE TEXT-TO-SPEECH
        private async void OnPlayPauseButtonClicked(object sender, EventArgs e)
        {
            if (_isPlaying)
            {
                StopAudio();
                return;
            }

            _isPlaying = true;
            _playPauseBtn.Opacity = 0.5;

            try
            {
                // Truyền isManual = true vì người dùng tự bấm tay
                await NarrationEngine.Instance.PlayNarrationAsync(_currentPlace, isManual: true);
            }
            catch (Exception ex)
            {
                ModalErrorHandler.Instance.HandleError(ex);
            }
            finally
            {
                // Khi đọc xong (hoặc bị hủy), tự động khôi phục lại nút bấm
                _isPlaying = false;
                _playPauseBtn.Opacity = 1.0;
            }
        }

        private void StopAudio()
        {
            NarrationEngine.Instance.CancelCurrentNarration();
            _isPlaying = false;
            _playPauseBtn.Opacity = 1.0;
        }
    }
}