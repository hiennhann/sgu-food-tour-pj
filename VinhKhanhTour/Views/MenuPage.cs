using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace VinhKhanhTour.Views
{
    public class MenuPage : ContentPage
    {
        public MenuPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#F8F9FA");

            var mainStack = new VerticalStackLayout { Spacing = 0 };

            // 1. Header có nút Back
            var headerGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 50 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 50 } }, Padding = new Thickness(10, 40, 10, 15), BackgroundColor = Colors.White };
            var backBtn = new ImageButton { Source = "icon_back_black.png", WidthRequest = 24, HeightRequest = 24 }; // Nhớ dùng icon màu đen
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            Grid.SetColumn(backBtn, 0);
            headerGrid.Children.Add(backBtn);

            var titleLabel = new Label { Text = "Thực Đơn", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(titleLabel, 1);
            headerGrid.Children.Add(titleLabel);
            mainStack.Children.Add(headerGrid);

            // 2. Danh sách món ăn
            var scrollContent = new ScrollView();
            var listStack = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(15) };

            listStack.Children.Add(CreateFoodItem("Ốc Hương Xào Bơ Tỏi", "120.000đ", "Nổi bật, thơm lừng bơ tỏi", "oc_huong_bo_toi.jpg"));
            listStack.Children.Add(CreateFoodItem("Sò Điệp Nướng Mỡ Hành", "90.000đ", "Sò điệp tươi, đậu phộng rang", "featured_food_1.jpg"));
            listStack.Children.Add(CreateFoodItem("Nghêu Hấp Sả", "70.000đ", "Nước dùng cay nồng vị sả ớt", "featured_food_2.jpg"));
            listStack.Children.Add(CreateFoodItem("Mì Xào Hải Sản", "85.000đ", "Tôm, mực, rau cải ngọt", "oc_vu_dish.jpg"));

            scrollContent.Content = listStack;
            mainStack.Children.Add(scrollContent);

            Content = mainStack;
        }

        private Border CreateFoodItem(string name, string price, string desc, string imageSrc)
        {
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 100 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 40 } }, ColumnSpacing = 15 };

            // Ảnh món ăn
            var imageBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 }, Content = new Image { Source = imageSrc, Aspect = Aspect.AspectFill }, Stroke = Colors.Transparent, HeightRequest = 100 };
            Grid.SetColumn(imageBorder, 0);
            grid.Children.Add(imageBorder);

            // Thông tin món
            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 5 };
            infoStack.Children.Add(new Label { Text = name, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            infoStack.Children.Add(new Label { Text = desc, FontSize = 12, TextColor = Colors.Gray, MaxLines = 2 });
            infoStack.Children.Add(new Label { Text = price, FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FF5C0F") });
            Grid.SetColumn(infoStack, 1);
            grid.Children.Add(infoStack);

            // Nút Thêm (Giả lập)
            var addBtn = new ImageButton { Source = "icon_add_orange.png", WidthRequest = 30, HeightRequest = 30, VerticalOptions = LayoutOptions.End, HorizontalOptions = LayoutOptions.End };
            Grid.SetColumn(addBtn, 2);
            grid.Children.Add(addBtn);

            return new Border { BackgroundColor = Colors.White, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Stroke = Colors.Transparent, Padding = new Thickness(10), Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 2) }, Content = grid };
        }
    }
}