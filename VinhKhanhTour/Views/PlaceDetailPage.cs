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

            var rootGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // ==========================================
            // PHẦN 1: NỘI DUNG CUỘN (SCROLLVIEW)
            // ==========================================
            var scrollContent = new ScrollView();
            var mainStack = new VerticalStackLayout { Spacing = 0 };

            var coverGrid = new Grid { HeightRequest = 340 };

            var coverImage = new Image { Aspect = Aspect.AspectFill };
            coverImage.SetBinding(Image.SourceProperty, "ImageUrl");
            coverGrid.Children.Add(coverImage);

            coverGrid.Children.Add(new BoxView
            {
                BackgroundColor = Color.FromArgb("#60000000"),
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 120
            });

            // NÚT BACK (Đã gắn Binding đa ngôn ngữ)
            var backBtn = new Button
            {
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#80000000"),
                HeightRequest = 40,
                CornerRadius = 20,
                Padding = new Thickness(20, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(15, 50, 0, 0)
            };
            backBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Quay lại"));
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            coverGrid.Children.Add(backBtn);
            mainStack.Children.Add(coverGrid);

            // --- KHUNG THÔNG TIN CHI TIẾT ---
            var infoLayout = new VerticalStackLayout { Spacing = 18, Padding = new Thickness(25, 30, 25, 40) };

            var nameLabel = new Label { FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            nameLabel.SetBinding(Label.TextProperty, "Name");
            infoLayout.Children.Add(nameLabel);

            var metaStack = new VerticalStackLayout { Spacing = 10 };

            var ratingStack = new HorizontalStackLayout { Spacing = 6 };
            ratingStack.Children.Add(new Label { Text = "⭐⭐⭐⭐⭐", FontSize = 16, TextColor = Color.FromArgb("#FFC107"), VerticalOptions = LayoutOptions.Center });
            var ratingLabel = new Label { FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center };
            ratingLabel.SetBinding(Label.TextProperty, new Binding("Rating"));
            ratingStack.Children.Add(ratingLabel);
            metaStack.Children.Add(ratingStack);

            var addressLabel = new Label { TextColor = Color.FromArgb("#707070"), FontSize = 15 };
            addressLabel.SetBinding(Label.TextProperty, new Binding("Address", stringFormat: "📍 {0}"));
            metaStack.Children.Add(addressLabel);

            // GIỜ MỞ CỬA (Đã gắn Binding đa ngôn ngữ)
            var timeLabel = new Label { TextColor = Color.FromArgb("#707070"), FontSize = 15 };
            timeLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Mở cửa", stringFormat: "🕒 {0}: 15:00 - 23:30"));
            metaStack.Children.Add(timeLabel);

            infoLayout.Children.Add(metaStack);

            infoLayout.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#EAEAEA"), Margin = new Thickness(0, 5) });

            var introLabel = new Label { FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            introLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Giới thiệu"));
            infoLayout.Children.Add(introLabel);

            var descLabel = new Label { TextColor = Color.FromArgb("#4A4A4A"), LineHeight = 1.5, FontSize = 16 };
            descLabel.SetBinding(Label.TextProperty, "NarrationText");
            infoLayout.Children.Add(descLabel);

            var infoBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(35, 35, 0, 0) },
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
                Shadow = new Shadow { Brush = Colors.Black, Radius = 10, Opacity = 0.08f, Offset = new Point(0, -4) }
            };

            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };

            var menuBtn = new Button { BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 55 };
            menuBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Thực Đơn", stringFormat: "📋 {0}"));
            menuBtn.Clicked += async (s, e) => await Navigation.PushAsync(new MenuPage(place));
            Grid.SetColumn(menuBtn, 0);
            actionGrid.Children.Add(menuBtn);

            var audioBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 55 };
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