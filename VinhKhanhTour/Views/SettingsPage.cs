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

            // 1. Tiêu đề Cài đặt (Đã trói buộc vào CurrentLanguageCode)
            var titleLabel = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.Black };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Cài đặt"));
            contentStack.Children.Add(titleLabel);

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

            // 3. Danh sách Menu Cài đặt
            var menuStack = new VerticalStackLayout { Spacing = 2, Margin = new Thickness(15, 10), BackgroundColor = Colors.White };

            // Nút Chọn Ngôn ngữ
            menuStack.Children.Add(CreateSettingsRow("Ngôn ngữ / Language", "icon_language.png", "Chạm để đổi", async () => {
                string action = await DisplayActionSheet("Chọn ngôn ngữ (Select Language)", "Hủy", null,
                    "Tiếng Việt", "English (Tiếng Anh)", "Español (Tiếng Tây Ban Nha)",
                    "Français (Tiếng Pháp)", "Deutsch (Tiếng Đức)", "中文 (Tiếng Trung)",
                    "日本語 (Tiếng Nhật)", "한국어 (Tiếng Hàn)", "Русский (Tiếng Nga)",
                    "Italiano (Tiếng Ý)", "Português (Tiếng Bồ Đào Nha)", "हिन्दी (Tiếng Hindi)");

                void ChangeAppLang(string cultureCode, string shortCode)
                {
                    LocalizationResourceManager.Instance.SetCulture(new CultureInfo(cultureCode));
                    AppState.ChangeLanguage(shortCode);
                }

                switch (action)
                {
                    case "Tiếng Việt": ChangeAppLang("vi-VN", "vi"); break;
                    case "English (Tiếng Anh)": ChangeAppLang("en-US", "en"); break;
                    case "Español (Tiếng Tây Ban Nha)": ChangeAppLang("es-ES", "es"); break;
                    case "Français (Tiếng Pháp)": ChangeAppLang("fr-FR", "fr"); break;
                    case "Deutsch (Tiếng Đức)": ChangeAppLang("de-DE", "de"); break;
                    case "中文 (Tiếng Trung)": ChangeAppLang("zh-CN", "zh"); break;
                    case "日本語 (Tiếng Nhật)": ChangeAppLang("ja-JP", "ja"); break;
                    case "한국어 (Tiếng Hàn)": ChangeAppLang("ko-KR", "ko"); break;
                    case "Русский (Tiếng Nga)": ChangeAppLang("ru-RU", "ru"); break;
                    case "Italiano (Tiếng Ý)": ChangeAppLang("it-IT", "it"); break;
                    case "Português (Tiếng Bồ Đào Nha)": ChangeAppLang("pt-PT", "pt"); break;
                    case "हिन्दी (Tiếng Hindi)": ChangeAppLang("hi-IN", "hi"); break;
                }
            }));

            var menuBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, BackgroundColor = Colors.White, Content = menuStack, Stroke = Colors.Transparent, Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 2) } };
            contentStack.Children.Add(menuBorder);

            scrollContent.Content = contentStack;
            mainGrid.Children.Add(scrollContent);

            // --- TAB BAR ---
            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 1);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        // HÀM TẠO DÒNG CÀI ĐẶT ĐÃ ĐƯỢC CHỈ ĐỊNH PATH CỤ THỂ
        private Grid CreateSettingsRow(string title, string icon, string value = "", Action onTap = null)
        {
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 40 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } }, Padding = new Thickness(15, 15) };
            grid.Children.Add(new Image { Source = icon, WidthRequest = 24, HeightRequest = 24, VerticalOptions = LayoutOptions.Center });

            // Trói buộc Tiêu đề (Dùng path: "CurrentLanguageCode")
            var titleLabel = new Label { TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: title));
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            var valueStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };

            if (!string.IsNullOrEmpty(value))
            {
                // Trói buộc cả chữ nhỏ (Ví dụ: "Chạm để đổi")
                var valLabel = new Label { TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center };
                valLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: value));
                valueStack.Children.Add(valLabel);
            }

            valueStack.Children.Add(new Label { Text = ">", TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            Grid.SetColumn(valueStack, 2);
            grid.Children.Add(valueStack);

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

            var textLabel = new Label { TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#808080"), FontSize = 10, HorizontalOptions = LayoutOptions.Center };
            // Sửa path thành "CurrentLanguageCode" ở Tab Bar
            textLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: text));
            layout.Children.Add(textLabel);

            return layout;
        }
    }
}