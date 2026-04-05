using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Views
{
    public class PlaceDetailPage : ContentPage
    {
        public PlaceDetailPage(FoodPlace place)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;
            BindingContext = place;

            // Dùng Grid chia màn hình: Phần trên là nội dung cuộn, Phần dưới là Thanh nút bấm cố định
            var rootGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star }, // Vùng cuộn
                    new RowDefinition { Height = GridLength.Auto }  // Thanh công cụ đáy
                }
            };

            // ==========================================
            // PHẦN 1: NỘI DUNG CUỘN (SCROLLVIEW)
            // ==========================================
            var scrollContent = new ScrollView();
            var mainStack = new VerticalStackLayout { Spacing = 0 };

            // --- ẢNH COVER TRÀN VIỀN ---
            var coverGrid = new Grid { HeightRequest = 320 }; // Tăng chiều cao ảnh bìa

            var coverImage = new Image { Aspect = Aspect.AspectFill };
            coverImage.SetBinding(Image.SourceProperty, "ImageUrl");
            coverGrid.Children.Add(coverImage);

            // Hiệu ứng Gradient mờ dần từ trên xuống để làm nổi bật nút Back
            coverGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#40000000"), VerticalOptions = LayoutOptions.Start, HeightRequest = 100 });

            // NÚT BACK (Dùng ký tự ← để chống tàng hình)
            var backBtn = new Button
            {
                Text = "←",
                FontSize = 28,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#40000000"),
                WidthRequest = 44,
                HeightRequest = 44,
                CornerRadius = 22,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(15, 40, 0, 0)
            };
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            coverGrid.Children.Add(backBtn);
            mainStack.Children.Add(coverGrid);

            // --- KHUNG THÔNG TIN CHI TIẾT (KÉO LÊN CHỒNG VÀO ẢNH BÌA) ---
            var infoLayout = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(25, 30, 25, 30) };

            // Tên Quán siêu to khổng lồ
            var nameLabel = new Label { FontSize = 30, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            nameLabel.SetBinding(Label.TextProperty, "Name");
            infoLayout.Children.Add(nameLabel);

            // Khung Đánh giá & Địa chỉ
            var metaStack = new VerticalStackLayout { Spacing = 8 };

            var ratingStack = new HorizontalStackLayout { Spacing = 5 };
            ratingStack.Children.Add(new Label { Text = "⭐⭐⭐⭐⭐", FontSize = 16, TextColor = Color.FromArgb("#FFC107"), VerticalOptions = LayoutOptions.Center });
            var ratingLabel = new Label { FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center };
            ratingLabel.SetBinding(Label.TextProperty, new Binding("Rating"));
            ratingStack.Children.Add(ratingLabel);
            metaStack.Children.Add(ratingStack);

            var addressLabel = new Label { TextColor = Color.FromArgb("#808080"), FontSize = 14 };
            addressLabel.SetBinding(Label.TextProperty, new Binding("Address", stringFormat: "📍 {0}"));
            metaStack.Children.Add(addressLabel);

            metaStack.Children.Add(new Label { Text = "🕒 Mở cửa: 15:00 - 23:30", TextColor = Color.FromArgb("#808080"), FontSize = 14 });
            infoLayout.Children.Add(metaStack);

            // Đường kẻ ngang phân cách
            infoLayout.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#F0F0F0"), Margin = new Thickness(0, 15) });

            // Phần Giới Thiệu
            var introLabel = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            introLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Giới thiệu"));
            infoLayout.Children.Add(introLabel);

            var descLabel = new Label { TextColor = Color.FromArgb("#505050"), LineHeight = 1.6, FontSize = 15 };
            descLabel.SetBinding(Label.TextProperty, "NarrationText");
            infoLayout.Children.Add(descLabel);

            // Kéo nguyên khối infoLayout lên 40px để đè lên ảnh Cover
            var infoBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(30, 30, 0, 0) },
                BackgroundColor = Colors.White,
                Content = infoLayout,
                Stroke = Colors.Transparent,
                Margin = new Thickness(0, -40, 0, 0)
            };
            mainStack.Children.Add(infoBorder);

            scrollContent.Content = mainStack;
            Grid.SetRow(scrollContent, 0);
            rootGrid.Children.Add(scrollContent);

            // ==========================================
            // PHẦN 2: THANH CÔNG CỤ ĐÁY CỐ ĐỊNH
            // ==========================================
            var bottomBar = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Padding = new Thickness(20, 15, 20, 25),
                Shadow = new Shadow { Brush = Colors.Black, Radius = 15, Opacity = 0.1f, Offset = new Point(0, -5) }
            };

            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };

            var menuBtn = new Button { BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 50 };
            menuBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Thực Đơn", stringFormat: "📋 {0}"));
            menuBtn.Clicked += async (s, e) => await Navigation.PushAsync(new MenuPage(place));
            Grid.SetColumn(menuBtn, 0);
            actionGrid.Children.Add(menuBtn);

            var audioBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 50 };
            audioBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "🎧 {0}"));
            audioBtn.Clicked += async (s, e) => await Navigation.PushAsync(new AudioPlayerPage(place));
            Grid.SetColumn(audioBtn, 1);
            actionGrid.Children.Add(audioBtn);

            bottomBar.Content = actionGrid;
            Grid.SetRow(bottomBar, 1);
            rootGrid.Children.Add(bottomBar);

            Content = rootGrid;
        }
    }
}