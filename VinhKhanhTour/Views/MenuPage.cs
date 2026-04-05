using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Views
{
    public class MenuPage : ContentPage
    {
        public MenuPage(FoodPlace place)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#F4F4F6"); // Nền xám nhạt để các thẻ món ăn nổi bật lên

            var mainStack = new VerticalStackLayout { Spacing = 0 };

            // ==========================================
            // 1. HEADER (Cố định phía trên)
            // ==========================================
            var headerGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 60 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 60 } },
                Padding = new Thickness(10, 50, 10, 15),
                BackgroundColor = Colors.White
            };

            // Nút Back dùng ký tự mũi tên
            var backBtn = new Button { Text = "←", FontSize = 28, TextColor = Colors.Black, BackgroundColor = Colors.Transparent, Padding = 0, WidthRequest = 40, HeightRequest = 40, CornerRadius = 20 };
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            Grid.SetColumn(backBtn, 0);
            headerGrid.Children.Add(backBtn);

            // Tiêu đề: Đổi thành Thực đơn tham khảo
            var titleLabel = new Label { Text = $"Thực đơn tham khảo", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(titleLabel, 1);
            headerGrid.Children.Add(titleLabel);

            // Đổ bóng cho Header
            var headerBorder = new Border { StrokeThickness = 0, BackgroundColor = Colors.White, Content = headerGrid, Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 4), Radius = 8 } };
            mainStack.Children.Add(headerBorder);

            // ==========================================
            // 2. DANH SÁCH MÓN ĂN (Scroll)
            // ==========================================
            var scrollContent = new ScrollView();
            var listStack = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(15, 20, 15, 40) };

            // Thêm dòng ghi chú cho khách du lịch
            var noteLabel = new Label
            {
                Text = "💡 Giá cả và món ăn chỉ mang tính chất tham khảo, có thể thay đổi tùy thời điểm thực tế.",
                FontSize = 13,
                TextColor = Colors.Gray,
                FontAttributes = FontAttributes.Italic,
                Margin = new Thickness(5, 0, 5, 10),
                HorizontalTextAlignment = TextAlignment.Center
            };
            listStack.Children.Add(noteLabel);

            // Thêm các món ăn
            listStack.Children.Add(CreateFoodItem("Ốc Hương Xào Bơ Tỏi", "120.000đ", "Nổi bật, thơm lừng bơ tỏi, chấm bánh mì cực cuốn.", "oc_huong_bo_toi.jpg"));
            listStack.Children.Add(CreateFoodItem("Sò Điệp Nướng Mỡ Hành", "90.000đ", "Sò điệp tươi rói, đậu phộng rang giòn rụm.", "featured_food_1.jpg"));
            listStack.Children.Add(CreateFoodItem("Nghêu Hấp Sả", "70.000đ", "Nước dùng cay nồng vị sả ớt, ấm bụng ngày mưa.", "featured_food_2.jpg"));
            listStack.Children.Add(CreateFoodItem("Mì Xào Hải Sản", "85.000đ", "Tôm, mực tươi giòn xào cùng mì và rau cải ngọt.", "oc_vu_dish.jpg"));

            scrollContent.Content = listStack;
            mainStack.Children.Add(scrollContent);

            Content = mainStack;
        }

        private Border CreateFoodItem(string name, string price, string desc, string imageSrc)
        {
            // BỎ CỘT THỨ 3 (Cột chứa nút Add)
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 90 }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };

            // Ảnh món ăn (Khung bo tròn vuông vức hiện đại)
            var imageBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }, Content = new Image { Source = imageSrc, Aspect = Aspect.AspectFill }, BackgroundColor = Color.FromArgb("#EAEAEA"), Stroke = Colors.Transparent, HeightRequest = 90, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(imageBorder, 0);
            grid.Children.Add(imageBorder);

            // Thông tin món (Giờ sẽ được giãn rộng ra tới sát mép lề phải)
            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 4 };
            infoStack.Children.Add(new Label { Text = name, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            infoStack.Children.Add(new Label { Text = desc, FontSize = 13, TextColor = Color.FromArgb("#808080"), LineHeight = 1.3, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation });
            infoStack.Children.Add(new Label { Text = price, FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FF5C0F"), Margin = new Thickness(0, 4, 0, 0) });
            Grid.SetColumn(infoStack, 1);
            grid.Children.Add(infoStack);

            // Bọc toàn bộ vào một chiếc thẻ trắng bo góc, có đổ bóng nhẹ
            return new Border { BackgroundColor = Colors.White, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Stroke = Colors.Transparent, Padding = new Thickness(12), Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.04f, Offset = new Point(0, 4), Radius = 10 }, Content = grid };
        }
    }
}