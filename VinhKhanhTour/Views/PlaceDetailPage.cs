using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Models; // Gọi thư mục Models để nhận diện FoodPlace

namespace VinhKhanhTour.Views
{
    public class PlaceDetailPage : ContentPage
    {
        // 1. NHẬN DỮ LIỆU TỪ TRANG CHỦ TRUYỀN SANG
        public PlaceDetailPage(FoodPlace place)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            // 2. GÁN DỮ LIỆU ĐÓ CHO TOÀN BỘ TRANG NÀY
            BindingContext = place;

            var scrollContent = new ScrollView();
            var mainStack = new VerticalStackLayout { Spacing = 0 };

            // --- ẢNH COVER VÀ NÚT BACK ---
            var coverGrid = new Grid { HeightRequest = 250 };

            // Trói buộc Ảnh Cover
            var coverImage = new Image { Aspect = Aspect.AspectFill };
            coverImage.SetBinding(Image.SourceProperty, "ImageUrl");
            coverGrid.Children.Add(coverImage);

            coverGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#40000000") });

            var backBtn = new ImageButton { Source = "icon_back_white.png", WidthRequest = 30, HeightRequest = 30, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(15, 40, 0, 0) };
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            coverGrid.Children.Add(backBtn);
            mainStack.Children.Add(coverGrid);

            // --- KHUNG THÔNG TIN CHI TIẾT ---
            var infoLayout = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(20), BackgroundColor = Colors.White };

            var titleRow = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } } };

            // Trói buộc Tên quán
            var titleLabel = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            titleLabel.SetBinding(Label.TextProperty, "Name");
            titleRow.Children.Add(titleLabel);

            // Trói buộc Đánh giá (Kèm định dạng thêm icon Ngôi sao)
            var ratingLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FF5C0F"), VerticalOptions = LayoutOptions.Center };
            ratingLabel.SetBinding(Label.TextProperty, new Binding("Rating", stringFormat: "⭐ {0}"));
            Grid.SetColumn(ratingLabel, 1);
            titleRow.Children.Add(ratingLabel);

            infoLayout.Children.Add(titleRow);

            // Trói buộc Địa chỉ
            var addressLabel = new Label { TextColor = Colors.DarkGray, FontSize = 14 };
            addressLabel.SetBinding(Label.TextProperty, new Binding("Address", stringFormat: "📍 {0}"));
            infoLayout.Children.Add(addressLabel);

            infoLayout.Children.Add(new Label { Text = "🕒 Mở cửa: 15:00 - 23:30", TextColor = Colors.DarkGray, FontSize = 14 });

            infoLayout.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#E0E0E0"), Margin = new Thickness(0, 10) });

            infoLayout.Children.Add(new Label { Text = "Giới thiệu", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });

            // Trói buộc Mô tả (Tạm thời lấy kịch bản NarrationText làm mô tả quán)
            var descLabel = new Label { TextColor = Colors.Gray, LineHeight = 1.4 };
            descLabel.SetBinding(Label.TextProperty, "NarrationText");
            infoLayout.Children.Add(descLabel);

            // --- HAI NÚT HÀNH ĐỘNG ---
            var actionGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15, Margin = new Thickness(0, 20) };

            var menuBtn = new Button { Text = "📋 Xem Thực Đơn", BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 15, HeightRequest = 55 };
            // menuBtn.Clicked += async (s, e) => await Navigation.PushAsync(new MenuPage()); 
            Grid.SetColumn(menuBtn, 0);
            actionGrid.Children.Add(menuBtn);

            var audioBtn = new Button { Text = "🎧 Nghe Audio", BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 15, HeightRequest = 55 };
            // audioBtn.Clicked += async (s, e) => await Navigation.PushAsync(new AudioPlayerPage()); 
            Grid.SetColumn(audioBtn, 1);
            actionGrid.Children.Add(audioBtn);

            infoLayout.Children.Add(actionGrid);

            var infoBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(25, 25, 0, 0) }, BackgroundColor = Colors.White, Content = infoLayout, Stroke = Colors.Transparent, Margin = new Thickness(0, -25, 0, 0) };
            mainStack.Children.Add(infoBorder);

            scrollContent.Content = mainStack;
            Content = scrollContent;
        }
    }
}