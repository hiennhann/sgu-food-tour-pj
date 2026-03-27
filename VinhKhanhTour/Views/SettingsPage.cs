using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System; // Cần thêm thư viện này để dùng Action
using VinhKhanhTour.Helpers;

namespace VinhKhanhTour.Views
{
    public class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#F8F9FA");

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 80 }
                }
            };

            var scrollContent = new ScrollView();
            var contentStack = new VerticalStackLayout { Spacing = 20, Padding = new Thickness(0, 40, 0, 20) };

            // 1. Tiêu đề
            contentStack.Children.Add(new Label { Text = "Cài đặt", FontSize = 24, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.Black });

            // 2. Khu vực Profile
            var profileLayout = new VerticalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };

            var avatarBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 40 },
                Stroke = Colors.Transparent,
                WidthRequest = 80,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.Center,
                Content = new Image { Source = "icon_avatar_placeholder.png", Aspect = Aspect.AspectFill }
            };
            profileLayout.Children.Add(avatarBorder);

            profileLayout.Children.Add(new Label { Text = "Nhân Nguyễn", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.Black });
            profileLayout.Children.Add(new Label { Text = "nhan.nguyen@email.com", FontSize = 14, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center });
            contentStack.Children.Add(profileLayout);

            // 3. Danh sách Menu Cài đặt (ĐÃ THÊM TƯƠNG TÁC)
            var menuStack = new VerticalStackLayout { Spacing = 2, Margin = new Thickness(15, 10), BackgroundColor = Colors.White };

            // Nút Tài khoản
            menuStack.Children.Add(CreateSettingsRow("Tài khoản của tôi", "icon_profile.png", "", async () => {
                await DisplayAlert("Tài khoản", "Tính năng quản lý tài khoản đang được phát triển.", "OK");
            }));

            // Nút Chọn Ngôn ngữ (Sử dụng ActionSheet để chọn)
            menuStack.Children.Add(CreateSettingsRow("Ngôn ngữ Thuyết minh", "icon_language.png", "Chạm để đổi", async () => {
                string action = await DisplayActionSheet("Chọn ngôn ngữ (Select Language)", "Hủy", null, "Tiếng Việt", "English", "한국어 (Tiếng Hàn)");

                if (action == "Tiếng Việt") AppState.ChangeLanguage("vi");
                else if (action == "English") AppState.ChangeLanguage("en");
                else if (action == "한국어 (Tiếng Hàn)") AppState.ChangeLanguage("ko");

                if (action != "Hủy" && !string.IsNullOrEmpty(action))
                {
                    await DisplayAlert("Thành công", $"Ngôn ngữ dữ liệu đã chuyển sang: {action}", "OK");
                }
            }));

            // Nút Thông báo
            menuStack.Children.Add(CreateSettingsRow("Thông báo", "icon_bell.png", "", async () => {
                await DisplayAlert("Thông báo", "Bạn không có thông báo mới nào hôm nay.", "Đóng");
            }));

            // Nút Chính sách bảo mật
            menuStack.Children.Add(CreateSettingsRow("Chính sách bảo mật", "icon_shield.png", "", async () => {
                await DisplayAlert("Bảo mật", "Dữ liệu của bạn được mã hóa an toàn theo tiêu chuẩn.", "Đã hiểu");
            }));

            var menuBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, BackgroundColor = Colors.White, Content = menuStack, Stroke = Colors.Transparent, Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 2) } };
            contentStack.Children.Add(menuBorder);

            // 4. Nút Đăng xuất (Có xác nhận Có/Không)
            var logoutBtn = new Button { Text = "Đăng xuất", BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 15, HeightRequest = 50, Margin = new Thickness(15, 10) };

            logoutBtn.Clicked += async (s, e) => {
                bool confirm = await DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi ứng dụng không?", "Đăng xuất", "Hủy");
                if (confirm)
                {
                    await DisplayAlert("Tạm biệt!", "Bạn đã đăng xuất thành công.", "OK");
                    // Sau này bạn có thể dùng lệnh: await Navigation.PushAsync(new LoginScreen()); để đưa về trang đăng nhập
                }
            };
            contentStack.Children.Add(logoutBtn);

            scrollContent.Content = contentStack;
            mainGrid.Children.Add(scrollContent);

            // --- TAB BAR ---
            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 1);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        // HÀM TẠO DÒNG CÀI ĐẶT ĐÃ ĐƯỢC NÂNG CẤP (Thêm biến Action onTap)
        private Grid CreateSettingsRow(string title, string icon, string value = "", Action onTap = null)
        {
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 40 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } }, Padding = new Thickness(15, 15) };
            grid.Children.Add(new Image { Source = icon, WidthRequest = 24, HeightRequest = 24, VerticalOptions = LayoutOptions.Center }); // Cột 0

            var titleLabel = new Label { Text = title, TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            var valueStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
            if (!string.IsNullOrEmpty(value)) valueStack.Children.Add(new Label { Text = value, TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center });
            valueStack.Children.Add(new Label { Text = ">", TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            Grid.SetColumn(valueStack, 2);
            grid.Children.Add(valueStack);

            // GẮN SỰ KIỆN CHẠM VÀO NẾU CÓ TRUYỀN HÀNH ĐỘNG (onTap)
            if (onTap != null)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => onTap.Invoke();
                grid.GestureRecognizers.Add(tapGesture);
            }

            return grid;
        }

        private Border CreateTabBar()
        {
            var tabBarGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, BackgroundColor = Colors.White, Padding = new Thickness(0, 5, 0, 5) };

            var tab1 = CreateTabItem("Trang chủ", "icon_home.png", false);
            var tapHome = new TapGestureRecognizer();
            tapHome.Tapped += async (s, e) => await Navigation.PushAsync(new HomePage(), false);
            tab1.GestureRecognizers.Add(tapHome);
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "icon_map.png", false);
            var tapMap = new TapGestureRecognizer();
            tapMap.Tapped += async (s, e) => await Navigation.PushAsync(new MapPage(), false);
            tab2.GestureRecognizers.Add(tapMap);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "icon_settings.png", true);
            Grid.SetColumn(tab3, 2);
            tabBarGrid.Children.Add(tab3);

            return new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(15, 15, 0, 0) }, BackgroundColor = Colors.White, Content = tabBarGrid, Stroke = Color.FromArgb("#E0E0E0") };
        }

        private View CreateTabItem(string text, string icon, bool isSelected = false)
        {
            var layout = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            layout.Children.Add(new Image { Source = icon, HeightRequest = 24, WidthRequest = 24, Opacity = isSelected ? 1.0 : 0.5 });
            layout.Children.Add(new Label { Text = text, TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#808080"), FontSize = 10, HorizontalOptions = LayoutOptions.Center });
            return layout;
        }
    }
}