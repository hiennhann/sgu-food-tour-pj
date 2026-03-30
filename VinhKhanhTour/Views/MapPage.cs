using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using VinhKhanhTour.Helpers; // Dùng TranslateConverter
using VinhKhanhTour.Models;
using VinhKhanhTour.Services; // Dùng LocalizationResourceManager

namespace VinhKhanhTour.Views
{
    public class MapPage : ContentPage
    {
        public MapPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 60 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 80 }
                }
            };

            var headerLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            // DỊCH TIÊU ĐỀ BẢN ĐỒ
            headerLabel.SetBinding(Label.TextProperty, new Binding(".", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Bản Đồ Phố Vĩnh Khánh"));
            Grid.SetRow(headerLabel, 0);
            mainGrid.Children.Add(headerLabel);

            var mapAreaGrid = new Grid();
            mapAreaGrid.Children.Add(new Image { Source = "map_background.jpg", Aspect = Aspect.AspectFill });

            var pinsLayout = new AbsoluteLayout();
            pinsLayout.Children.Add(CreateMapPin("1", 120, 150));
            pinsLayout.Children.Add(CreateMapPin("2", 100, 220));
            pinsLayout.Children.Add(CreateMapPin("3", 180, 280));
            mapAreaGrid.Children.Add(pinsLayout);

            var floatingControls = new VerticalStackLayout { HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0, 15, 15, 0), Spacing = 10 };

            var locationLayout = new HorizontalStackLayout { Spacing = 5 };
            locationLayout.Children.Add(new Image { Source = "icon_location_blue.png", WidthRequest = 16, HeightRequest = 16 });
            var locationLabel = new Label { TextColor = Color.FromArgb("#0066CC"), VerticalOptions = LayoutOptions.Center };
            locationLabel.SetBinding(Label.TextProperty, new Binding(".", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "BẠN ĐANG Ở GẦN"));
            locationLayout.Children.Add(locationLabel);
            floatingControls.Children.Add(new Border { Stroke = Color.FromArgb("#E0E0E0"), StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 }, BackgroundColor = Colors.White, Padding = new Thickness(10, 5), Content = locationLayout });

            mapAreaGrid.Children.Add(floatingControls);

            var bottomCard = CreateBottomInfoCard();
            mapAreaGrid.Children.Add(bottomCard);

            Grid.SetRow(mapAreaGrid, 1);
            mainGrid.Children.Add(mapAreaGrid);

            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 2);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        private View CreateMapPin(string number, double x, double y)
        {
            var pinBorder = new Border { WidthRequest = 24, HeightRequest = 32, BackgroundColor = Colors.Black, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(12, 12, 12, 0) }, Content = new Label { Text = number, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 12, FontAttributes = FontAttributes.Bold } };
            AbsoluteLayout.SetLayoutBounds(pinBorder, new Rect(x, y, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(pinBorder, AbsoluteLayoutFlags.None);
            return pinBorder;
        }

        private Border CreateBottomInfoCard()
        {
            var cardGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 100 }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };
            var foodImageBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 }, Stroke = Colors.Transparent, Content = new Image { Source = "oc_vu_dish.jpg", Aspect = Aspect.AspectFill } };
            Grid.SetColumn(foodImageBorder, 0);
            cardGrid.Children.Add(foodImageBorder);
            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 5 };
            infoStack.Children.Add(new Label { Text = "1. Quán Ốc Vũ - 37 Vĩnh Khánh", FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Black });

            var ratingStack = new HorizontalStackLayout { Spacing = 5 };
            ratingStack.Children.Add(new Label { Text = "4.8", TextColor = Colors.Black });
            ratingStack.Children.Add(new Label { Text = "⭐", TextColor = Colors.Gold });
            ratingStack.Children.Add(new Label { Text = "(154)", TextColor = Colors.Gray });
            infoStack.Children.Add(ratingStack);

            var audioBtn = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 8, HeightRequest = 40, Margin = new Thickness(0, 5, 0, 0) };
            audioBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "▶ {0}"));

            // --- THÊM PHẦN XỬ LÝ CLICK ---
            audioBtn.Clicked += async (s, e) =>
            {
                // Tạo một FoodPlace giả lập dựa trên dữ liệu hiện tại của thẻ
                var currentMapPlace = new FoodPlace
                {
                    Id = "1",
                    Name = "Quán Ốc Vũ",
                    // Mẹo: Gọi thẳng từ điển ra để đọc kịch bản đa ngôn ngữ
                    NarrationText = LocalizationResourceManager.Instance["Quán Ốc Vũ là một trong những quán ốc lâu đời và nổi tiếng nhất tại Vĩnh Khánh."] ?? "Quán Ốc Vũ..."
                };

                // Nhờ Engine phát âm thanh ngay trên bản đồ
                await NarrationEngine.Instance.PlayNarrationAsync(currentMapPlace, isManual: true);
            };

            infoStack.Children.Add(audioBtn);

            Grid.SetColumn(infoStack, 1);
            cardGrid.Children.Add(infoStack);

            return new Border { BackgroundColor = Colors.White, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Stroke = Color.FromArgb("#E0E0E0"), Padding = new Thickness(15), Margin = new Thickness(15, 0, 15, 15), VerticalOptions = LayoutOptions.End, Content = cardGrid };
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

            var tab2 = CreateTabItem("Bản đồ", "icon_map.png", true);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "icon_settings.png", false);
            var tapSettings = new TapGestureRecognizer();
            tapSettings.Tapped += async (s, e) => await Navigation.PushAsync(new SettingsPage(), false);
            tab3.GestureRecognizers.Add(tapSettings);
            Grid.SetColumn(tab3, 2);
            tabBarGrid.Children.Add(tab3);

            return new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(15, 15, 0, 0) }, BackgroundColor = Colors.White, Content = tabBarGrid, Stroke = Color.FromArgb("#E0E0E0") };
        }

        // HÀM TẠO TAB ĐÃ ĐƯỢC ĐỒNG BỘ
        private View CreateTabItem(string text, string icon, bool isSelected = false)
        {
            var layout = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            layout.Children.Add(new Image { Source = icon, HeightRequest = 24, WidthRequest = 24, Opacity = isSelected ? 1.0 : 0.5 });

            var textLabel = new Label { TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#808080"), FontSize = 10, HorizontalOptions = LayoutOptions.Center };
            textLabel.SetBinding(Label.TextProperty, new Binding(".", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: text));
            layout.Children.Add(textLabel);

            return layout;
        }
    }
}