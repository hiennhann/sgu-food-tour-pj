using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;
using VinhKhanhTour.Helpers;
using VinhKhanhTour.Services;

namespace VinhKhanhTour.Views
{
    public class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#F4F4F6"); // Nền xám nhạt để làm nổi bật các thẻ trắng

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 60 }, // Header
                    new RowDefinition { Height = GridLength.Star }, // Content
                    new RowDefinition { Height = 80 }  // Tab Bar
                }
            };

            // ==========================================
            // 1. HEADER KHUNG TRÊN CÙNG
            // ==========================================
            var headerGrid = new Grid { BackgroundColor = Colors.White, Padding = new Thickness(20, 0) };
            var titleLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 20, TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

            // Trói buộc Tiêu đề
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Cài đặt"));
            headerGrid.Children.Add(titleLabel);

            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);

            // ==========================================
            // 2. NỘI DUNG CUỘN CÀI ĐẶT
            // ==========================================
            var scrollContent = new ScrollView();
            var contentStack = new VerticalStackLayout { Spacing = 20, Padding = new Thickness(15, 20) };

            // --- THẺ PROFILE KHÁCH VÃNG LAI (GUEST) ---
            var profileCard = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                Padding = new Thickness(15),
                Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 2), Radius = 5 }
            };

            var profileGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 60 }, new ColumnDefinition { Width = GridLength.Star } },
                RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto } },
                ColumnSpacing = 15,
                RowSpacing = 5
            };

            // Avatar ẩn danh
            var avatar = new Border { WidthRequest = 60, HeightRequest = 60, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 30 }, BackgroundColor = Color.FromArgb("#E0E0E0"), StrokeThickness = 0, Content = new Label { Text = "👤", FontSize = 30, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center } };
            Grid.SetRowSpan(avatar, 2);
            Grid.SetColumn(avatar, 0);
            profileGrid.Children.Add(avatar);

            // Tên Khách
            var nameLabel = new Label { Text = "Khách du lịch", FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black, VerticalOptions = LayoutOptions.End };
            Grid.SetRow(nameLabel, 0);
            Grid.SetColumn(nameLabel, 1);
            profileGrid.Children.Add(nameLabel);

            // Dòng mời gọi
            var hintLabel = new Label { Text = "Đăng nhập để lưu địa điểm yêu thích", FontSize = 13, TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Start };
            Grid.SetRow(hintLabel, 1);
            Grid.SetColumn(hintLabel, 1);
            profileGrid.Children.Add(hintLabel);

            // Nút Đăng nhập / Đăng ký
            var loginBtn = new Button { Text = "Đăng Nhập / Đăng Ký", BackgroundColor = Color.FromArgb("#FFF0ED"), TextColor = Color.FromArgb("#FF5C0F"), FontAttributes = FontAttributes.Bold, CornerRadius = 10, HeightRequest = 40, Margin = new Thickness(0, 10, 0, 0) };
            loginBtn.Clicked += async (s, e) => await DisplayAlert("Thông báo", "Tính năng Đăng nhập đang được phát triển!", "Đóng");
            Grid.SetRow(loginBtn, 2);
            Grid.SetColumn(loginBtn, 1);
            profileGrid.Children.Add(loginBtn);

            profileCard.Content = profileGrid;
            contentStack.Children.Add(profileCard);

            // --- CỤM CÀI ĐẶT CHUNG ---
            var generalSection = new VerticalStackLayout { Spacing = 10 };
            var generalTitle = new Label { Text = "CÀI ĐẶT CHUNG", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Colors.Gray, Margin = new Thickness(5, 0, 0, 0) };
            generalSection.Children.Add(generalTitle);

            var generalCard = new Border { BackgroundColor = Colors.White, StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 } };
            var generalList = new VerticalStackLayout();

            // Mục Đổi Ngôn Ngữ
            var langRow = CreateSettingsRow("🌐", "Ngôn ngữ / Language", "Chạm để đổi", true);
            var tapLang = new TapGestureRecognizer();
            tapLang.Tapped += async (s, e) => await ChangeLanguageAsync();
            langRow.GestureRecognizers.Add(tapLang);
            generalList.Children.Add(langRow);

            // Đường kẻ ngang
            generalList.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#F0F0F0"), Margin = new Thickness(45, 0, 15, 0) });

            // Mục Âm thanh
            var audioRow = CreateSettingsRow("🎧", "Âm thanh & Giọng đọc", "Mặc định", true);
            generalList.Children.Add(audioRow);

            generalCard.Content = generalList;
            generalSection.Children.Add(generalCard);
            contentStack.Children.Add(generalSection);

            // --- CỤM THÔNG TIN HỖ TRỢ ---
            var aboutSection = new VerticalStackLayout { Spacing = 10 };
            var aboutTitle = new Label { Text = "THÔNG TIN & HỖ TRỢ", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Colors.Gray, Margin = new Thickness(5, 0, 0, 0) };
            aboutSection.Children.Add(aboutTitle);

            var aboutCard = new Border { BackgroundColor = Colors.White, StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 } };
            var aboutList = new VerticalStackLayout();

            aboutList.Children.Add(CreateSettingsRow("🛡️", "Chính sách bảo mật", "", true));
            aboutList.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#F0F0F0"), Margin = new Thickness(45, 0, 15, 0) });
            aboutList.Children.Add(CreateSettingsRow("ℹ️", "Về ứng dụng Vĩnh Khánh Tour", "Phiên bản 1.0.0", false));

            aboutCard.Content = aboutList;
            aboutSection.Children.Add(aboutCard);
            contentStack.Children.Add(aboutSection);

            // Bản quyền
            var footerLabel = new Label { Text = "Thiết kế ❤️ bởi Nhân Nguyễn", FontSize = 12, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 20) };
            contentStack.Children.Add(footerLabel);

            scrollContent.Content = contentStack;
            Grid.SetRow(scrollContent, 1);
            mainGrid.Children.Add(scrollContent);

            // ==========================================
            // 3. TAB BAR DƯỚI CÙNG
            // ==========================================
            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 2);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        // HÀM TẠO 1 DÒNG CÀI ĐẶT
        private View CreateSettingsRow(string icon, string titleKey, string valueText, bool showArrow)
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 40 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = 20 } },
                Padding = new Thickness(15, 15),
                BackgroundColor = Colors.Transparent
            };

            grid.Children.Add(new Label { Text = icon, FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start });

            var titleLabel = new Label { TextColor = Colors.Black, FontSize = 15, VerticalOptions = LayoutOptions.Center };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: titleKey));
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            if (!string.IsNullOrEmpty(valueText))
            {
                var valueLabel = new Label { TextColor = Colors.Gray, FontSize = 14, VerticalOptions = LayoutOptions.Center };
                // Trói buộc dữ liệu cho phần valueText (Ví dụ: "Chạm để đổi")
                valueLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: valueText));
                Grid.SetColumn(valueLabel, 2);
                grid.Children.Add(valueLabel);
            }

            if (showArrow)
            {
                var arrowLabel = new Label { Text = "›", TextColor = Colors.Silver, FontSize = 20, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
                Grid.SetColumn(arrowLabel, 3);
                grid.Children.Add(arrowLabel);
            }

            return grid;
        }

        // HÀM XỬ LÝ ĐỔI NGÔN NGỮ KÈM DANH SÁCH 12 TIẾNG
        private async Task ChangeLanguageAsync()
        {
            string action = await DisplayActionSheet("Chọn ngôn ngữ (Select Language)", "Hủy", null,
                "Tiếng Việt", "English (Tiếng Anh)", "Español (Tiếng Tây Ban Nha)",
                "Français (Tiếng Pháp)", "Deutsch (Tiếng Đức)", "中文 (Tiếng Trung)",
                "日本語 (Tiếng Nhật)", "한국어 (Tiếng Hàn)", "Русский (Tiếng Nga)",
                "Italiano (Tiếng Ý)", "Português (Tiếng Bồ Đào Nha)", "हिन्दी (Tiếng Hindi)");

            if (string.IsNullOrEmpty(action) || action == "Hủy") return;

            CultureInfo culture = action switch
            {
                "Tiếng Việt" => new CultureInfo("vi-VN"),
                "English (Tiếng Anh)" => new CultureInfo("en-US"),
                "Español (Tiếng Tây Ban Nha)" => new CultureInfo("es-ES"),
                "Français (Tiếng Pháp)" => new CultureInfo("fr-FR"),
                "Deutsch (Tiếng Đức)" => new CultureInfo("de-DE"),
                "中文 (Tiếng Trung)" => new CultureInfo("zh-CN"),
                "日本語 (Tiếng Nhật)" => new CultureInfo("ja-JP"),
                "한국어 (Tiếng Hàn)" => new CultureInfo("ko-KR"),
                "Русский (Tiếng Nga)" => new CultureInfo("ru-RU"),
                "Italiano (Tiếng Ý)" => new CultureInfo("it-IT"),
                "Português (Tiếng Bồ Đào Nha)" => new CultureInfo("pt-PT"),
                "हिन्दी (Tiếng Hindi)" => new CultureInfo("hi-IN"),
                _ => new CultureInfo("vi-VN")
            };

            // Gửi lệnh cập nhật ngôn ngữ cho toàn bộ App
            LocalizationResourceManager.Instance.SetCulture(culture);
        }

        // ========================================================
        // TẠO TAB BAR BẰNG CODE
        // ========================================================
        // ========================================================
        // TẠO TAB BAR BẰNG CODE (GIAO DIỆN FLOATING PILL HIỆN ĐẠI)
        // ========================================================
        private Border CreateTabBar()
        {
            var tabBarGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                BackgroundColor = Colors.Transparent,
                Padding = new Thickness(0, 10, 0, 10)
            };

            var tab1 = CreateTabItem("Trang chủ", "🏠", false);
            var tapHome = new TapGestureRecognizer();
            tapHome.Tapped += async (s, e) => await Navigation.PushAsync(new HomePage(), false);
            tab1.GestureRecognizers.Add(tapHome);
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "🗺️", false);
            var tapMap = new TapGestureRecognizer();
            tapMap.Tapped += async (s, e) => await Navigation.PushAsync(new MapPage(), false);
            tab2.GestureRecognizers.Add(tapMap);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            // BẬT SÁNG NÚT CÀI ĐẶT VÀ BỎ SỰ KIỆN CLICK (Vì đang ở trang này rồi)
            var tab3 = CreateTabItem("Cài đặt", "⚙️", true);
            Grid.SetColumn(tab3, 2);
            tabBarGrid.Children.Add(tab3);

            return new Border
            {
                Margin = new Thickness(20, 0, 20, 15),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 25 },
                BackgroundColor = Colors.White,
                Content = tabBarGrid,
                Stroke = Colors.Transparent,
                Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.15f, Offset = new Point(0, 5), Radius = 15 }
            };
        }

        private View CreateTabItem(string text, string iconText, bool isSelected = false)
        {
            var layout = new VerticalStackLayout { Spacing = 4, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            var iconLabel = new Label
            {
                Text = iconText,
                FontSize = 22,
                HorizontalOptions = LayoutOptions.Center,
                Opacity = isSelected ? 1.0 : 0.4
            };
            layout.Children.Add(iconLabel);

            var textLabel = new Label
            {
                TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#808080"),
                FontSize = 11,
                FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
                HorizontalOptions = LayoutOptions.Center
            };
            textLabel.SetBinding(Label.TextProperty, new Binding(
                path: "CurrentLanguageCode",
                source: LocalizationResourceManager.Instance,
                converter: TranslateConverter.Instance,
                converterParameter: text
            ));
            layout.Children.Add(textLabel);

            return layout;
        }
    }
}