using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Media;

namespace VinhKhanhTour.Views
{
    public class AudioPlayerPage : ContentPage
    {
        public AudioPlayerPage()
        {
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

            // Nút Back
            var backBtn = new ImageButton { Source = "icon_back_white.png", WidthRequest = 24, HeightRequest = 24, HorizontalOptions = LayoutOptions.Center };
            Grid.SetColumn(backBtn, 0);
            headerGrid.Children.Add(backBtn);

            // Tiêu đề
            var titleLabel = new Label { Text = "Quán Ốc Vũ", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 18, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(titleLabel, 1);
            headerGrid.Children.Add(titleLabel);

            // Nút Share
            var shareBtn = new ImageButton { Source = "icon_share_white.png", WidthRequest = 24, HeightRequest = 24, HorizontalOptions = LayoutOptions.Center };
            Grid.SetColumn(shareBtn, 2);
            headerGrid.Children.Add(shareBtn);

            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);

            // --- HÀNG 2: NỘI DUNG CHÍNH ---
            var contentScrollView = new ScrollView();
            var contentStack = new VerticalStackLayout { Padding = new Thickness(15), Spacing = 20 };

            // 1. Ảnh món ăn
            var imageGrid = new Grid { HeightRequest = 220 };
            imageGrid.Children.Add(new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                Stroke = Colors.Transparent,
                Content = new Image { Source = "oc_huong_bo_toi.jpg", Aspect = Aspect.AspectFill }
            });

            var imageLabel = new Border
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10, 0, 15, 0) },
                Stroke = Colors.Transparent,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(10, 5),
                Content = new Label { Text = "Ốc Hương Xào Bơ Tỏi", TextColor = Colors.White, FontSize = 12 }
            };
            imageGrid.Children.Add(imageLabel);
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
            controlButtonsStack.Children.Add(new ImageButton { Source = "icon_rewind_10s.png", WidthRequest = 35, HeightRequest = 35 });
            controlButtonsStack.Children.Add(new ImageButton { Source = "icon_play_orange_circle.png", WidthRequest = 60, HeightRequest = 60 });
            controlButtonsStack.Children.Add(new ImageButton { Source = "icon_forward_10s.png", WidthRequest = 35, HeightRequest = 35 });
            playerStack.Children.Add(controlButtonsStack);

            // Thanh tiến trình
            var timeGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 40 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 40 } }, ColumnSpacing = 10 };

            var startTimeLabel = new Label { Text = "0:45", TextColor = Colors.White, FontSize = 12, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(startTimeLabel, 0);
            timeGrid.Children.Add(startTimeLabel);

            var timeSlider = new Slider { Minimum = 0, Maximum = 200, Value = 45, MinimumTrackColor = Color.FromArgb("#E68A00"), MaximumTrackColor = Color.FromArgb("#8B3A2B"), ThumbColor = Colors.White };
            Grid.SetColumn(timeSlider, 1);
            timeGrid.Children.Add(timeSlider);

            var durationLabel = new Label { Text = "3:20", TextColor = Colors.White, FontSize = 12, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
            Grid.SetColumn(durationLabel, 2);
            timeGrid.Children.Add(durationLabel);

            playerStack.Children.Add(timeGrid);

            // Thanh Volume
            var volumeGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 20 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 20 } }, ColumnSpacing = 10 };

            var volDownImage = new Image { Source = "icon_volume_down.png", WidthRequest = 16, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(volDownImage, 0);
            volumeGrid.Children.Add(volDownImage);

            var volSlider = new Slider { Minimum = 0, Maximum = 100, Value = 40, MinimumTrackColor = Color.FromArgb("#E68A00"), MaximumTrackColor = Color.FromArgb("#8B3A2B"), ThumbColor = Colors.White };
            Grid.SetColumn(volSlider, 1);
            volumeGrid.Children.Add(volSlider);

            var volUpImage = new Image { Source = "icon_volume_up.png", WidthRequest = 16, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(volUpImage, 2);
            volumeGrid.Children.Add(volUpImage);

            playerStack.Children.Add(volumeGrid);

            playerBorder.Content = playerStack;
            contentStack.Children.Add(playerBorder);

            // 3. Text Mô tả
            contentStack.Children.Add(new Label { Text = "Về Quán Ốc Vũ", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 18 });
            contentStack.Children.Add(new Label
            {
                Text = "Quán Ốc Vũ là một trong những quán ốc lâu đời và nổi tiếng nhất tại Vĩnh Khánh. Được biết đến với hương vị đậm đà, các món ốc tươi ngon và không khí sôi động. Đừng bỏ lỡ Ốc Hương Xào Bơ Tỏi và Sò Điệp Nướng Mỡ Hành.",
                TextColor = Color.FromArgb("#F0D6D0"),
                FontSize = 14,
                LineHeight = 1.4
            });

            // 4. Các nút Hành động
            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15, Margin = new Thickness(0, 10, 0, 20) };

            var routeBtn = new Button { Text = "📍 Chỉ Đường", BackgroundColor = Color.FromArgb("#D9801A"), TextColor = Colors.White, CornerRadius = 25, HeightRequest = 50 };
            Grid.SetColumn(routeBtn, 0);
            actionGrid.Children.Add(routeBtn);

            var menuBtn = new Button { Text = "📋 Xem Thực Đơn", BackgroundColor = Color.FromArgb("#7B4335"), TextColor = Colors.White, BorderColor = Color.FromArgb("#D9801A"), BorderWidth = 1, CornerRadius = 25, HeightRequest = 50 };
            Grid.SetColumn(menuBtn, 1);
            actionGrid.Children.Add(menuBtn);

            contentStack.Children.Add(actionGrid);

            contentScrollView.Content = contentStack;
            Grid.SetRow(contentScrollView, 1);
            mainGrid.Children.Add(contentScrollView);

            // --- HÀNG 3: THANH TAB BAR ---
            var tabBarBorder = CreateDarkTabBar();
            Grid.SetRow(tabBarBorder, 2);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        private Border CreateDarkTabBar()
        {
            var tabBarGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                BackgroundColor = Color.FromArgb("#3D1409"),
                Padding = new Thickness(0, 5, 0, 5)
            };

            var tab1 = CreateTabItem("Trang chủ", "icon_home.png", isSelected: false);
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "icon_map.png", isSelected: true);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "icon_settings.png", isSelected: false);
            Grid.SetColumn(tab3, 2);
            tabBarGrid.Children.Add(tab3);

            return new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(15, 15, 0, 0) },
                BackgroundColor = Color.FromArgb("#3D1409"),
                Content = tabBarGrid,
                Stroke = Colors.Transparent
            };
        }

        private View CreateTabItem(string text, string icon, bool isSelected = false)
        {
            var layout = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            layout.Children.Add(new Image
            {
                Source = icon,
                HeightRequest = 24,
                WidthRequest = 24,
                Opacity = isSelected ? 1.0 : 0.5 // Làm mờ các icon không được chọn thay vì dùng ColorFilter
            });
            layout.Children.Add(new Label
            {
                Text = text,
                TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#A3847C"),
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Center
            });

            return layout;
        }
    }
}