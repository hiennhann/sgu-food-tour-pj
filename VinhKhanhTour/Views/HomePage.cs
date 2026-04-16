using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Views;
using VinhKhanhTour.ViewModels;
using VinhKhanhTour.Models;
using System.Collections.Generic;

namespace VinhKhanhTour
{
    public class HomePage : ContentPage
    {
        // Chứa danh sách các nút bộ lọc để dễ đổi màu
        private List<Border> _categoryButtons = new List<Border>();

        public HomePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            BindingContext = new HomeViewModel();

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 80 }
                }
            };

            // ========================================================
            // 1. HEADER (CỐ ĐỊNH PHÍA TRÊN)
            // ========================================================
            var headerGrid = new Grid { HeightRequest = 260 };
            headerGrid.Children.Add(new Image { Source = "hero_vinh_khanh.jpg", Aspect = Aspect.AspectFill });
            headerGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#70000000") });

            var headerContentStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 12, Padding = new Thickness(20) };

            var titleLabel = new Label { FontSize = 26, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
            titleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Khám phá Ẩm thực"));
            headerContentStack.Children.Add(titleLabel);

            var subtitleLabel = new Label { FontSize = 14, TextColor = Colors.White, Opacity = 0.9, HorizontalTextAlignment = TextAlignment.Center };
            subtitleLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Thiên đường Ốc Sài Gòn — Quận 4"));
            headerContentStack.Children.Add(subtitleLabel);

            var playBtnHeader = new ImageButton { Source = "icon_play.png", BackgroundColor = Color.FromArgb("#FF5C0F"), WidthRequest = 50, HeightRequest = 50, CornerRadius = 25, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 10, 0, 0) };
            playBtnHeader.Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.5f, Offset = new Point(0, 4), Radius = 10 };
            headerContentStack.Children.Add(playBtnHeader);

            headerGrid.Children.Add(headerContentStack);
            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);

            // ========================================================
            // 2. TẠO KHUNG HEADER CHO DANH SÁCH CUỘN (CHỐNG LỖI TÀNG HÌNH)
            // ========================================================
            var headerForCollection = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(15, 15, 15, 10) };

            // Thanh tìm kiếm
            var searchBorder = new Border
            {
                Stroke = Color.FromArgb("#E5E5E5"),
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                BackgroundColor = Color.FromArgb("#F9F9F9"),
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(15, 0, 15, 0)
            };
            var searchGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            searchGrid.Children.Add(new Label { Text = "🔍", TextColor = Colors.Gray, VerticalOptions = LayoutOptions.Center, FontSize = 16, Margin = new Thickness(0, 0, 10, 0) });

            var searchEntry = new Entry { BackgroundColor = Colors.Transparent, HeightRequest = 45 };
            searchEntry.SetBinding(Entry.PlaceholderProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Tìm quán ốc, lẩu, sushi..."));
            Grid.SetColumn(searchEntry, 1);
            searchGrid.Children.Add(searchEntry);
            searchBorder.Content = searchGrid;
            headerForCollection.Children.Add(searchBorder);

            // Nút bấm lọc
            headerForCollection.Children.Add(CreateCategoryList());

            // Nút CTA Bắt đầu tour
            var ctaButton = new Button { BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 50, Margin = new Thickness(0, 10) };
            ctaButton.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Bắt Đầu Tour Ngay", stringFormat: "🗺️ {0} →"));
            ctaButton.Clicked += async (s, e) => await Navigation.PushAsync(new MapPage());
            headerForCollection.Children.Add(ctaButton);

            // Tiêu đề địa điểm nổi bật
            var featuredHeader = new HorizontalStackLayout { Spacing = 5 };
            featuredHeader.Children.Add(new Label { Text = "📍", TextColor = Color.FromArgb("#FF5C0F"), FontSize = 20 });
            var headerLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center };
            headerLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Địa điểm nổi bật"));
            featuredHeader.Children.Add(headerLabel);
            headerForCollection.Children.Add(featuredHeader);

            // =========================================================
            // 3. VẼ DANH SÁCH BẰNG COLLECTION VIEW
            // =========================================================
            var placesCollectionView = new CollectionView();
            placesCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "FeaturedPlaces");

            // Đưa toàn bộ Tìm kiếm, Bộ lọc vào Header của CollectionView (Cách chuẩn MAUI)
            placesCollectionView.Header = headerForCollection;

            // Thêm footer 100px để cuộn tới đáy không bị TabBar đè lên
            placesCollectionView.Footer = new ContentView { HeightRequest = 100 };

            // Trạng thái trống (Empty State)
            var emptyLayout = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Spacing = 10, Margin = new Thickness(0, 40, 0, 0) };
            emptyLayout.Children.Add(new Label { Text = "🍽️", FontSize = 40, HorizontalOptions = LayoutOptions.Center });
            var emptyLabel = new Label { TextColor = Colors.Gray, FontSize = 14, HorizontalOptions = LayoutOptions.Center };
            emptyLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Khu vực này chưa có quán nào"));
            emptyLayout.Children.Add(emptyLabel);
            placesCollectionView.EmptyView = emptyLayout;

            // Template thẻ Quán ăn
            placesCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var cardGrid = new Grid { HeightRequest = 180, Margin = new Thickness(15, 0, 15, 15) };

                var bgImage = new Image { Aspect = Aspect.AspectFill };
                bgImage.SetBinding(Image.SourceProperty, "ImageUrl");
                cardGrid.Children.Add(new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Content = bgImage, Stroke = Colors.Transparent });
                cardGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#30000000") });

                var titleLabelCard = new Label { TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 18, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(15, 15, 0, 0) };
                titleLabelCard.SetBinding(Label.TextProperty, "Name");
                cardGrid.Children.Add(titleLabelCard);

                var playBtn = new Button
                {
                    BackgroundColor = Colors.White,
                    TextColor = Color.FromArgb("#FF5C0F"),
                    BorderColor = Color.FromArgb("#FF5C0F"),
                    BorderWidth = 1,
                    FontSize = 10,
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(10, 5),
                    CornerRadius = 10,
                    HeightRequest = 30,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 15, 15)
                };
                playBtn.SetBinding(Button.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Nghe Audio", stringFormat: "▶️ {0}"));

                playBtn.Clicked += async (s, e) =>
                {
                    var btn = (Button)s;
                    var selectedPlace = (FoodPlace)btn.BindingContext;
                    if (selectedPlace != null)
                    {
                        VinhKhanhTour.Services.NarrationEngine.Instance.CancelCurrentNarration();
                        await VinhKhanhTour.Services.NarrationEngine.Instance.PlayNarrationAsync(selectedPlace, isManual: true);
                    }
                };

                cardGrid.Children.Add(playBtn);

                var headphoneIcon = new Image { Source = "icon_headphone.png", HeightRequest = 25, WidthRequest = 25, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0, 15, 15, 0) };
                cardGrid.Children.Add(headphoneIcon);

                var tapPlace = new TapGestureRecognizer();
                tapPlace.Tapped += async (s, e) =>
                {
                    var selectedPlace = (FoodPlace)((BindableObject)s).BindingContext;
                    await Navigation.PushAsync(new PlaceDetailPage(selectedPlace));
                };
                cardGrid.GestureRecognizers.Add(tapPlace);

                return cardGrid;
            });

            Grid.SetRow(placesCollectionView, 1);
            mainGrid.Children.Add(placesCollectionView);

            // ========================================================
            // 4. TAB BAR HIỆN ĐẠI
            // ========================================================
            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 2);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        // ============================================================
        // CÁC HÀM XỬ LÝ BỘ LỌC CATEGORY
        // ============================================================
        private View CreateCategoryList()
        {
            var stack = new HorizontalStackLayout { Spacing = 12, Padding = new Thickness(5, 5, 20, 15) };
            _categoryButtons.Clear();

            // Đã cập nhật đầy đủ 7 danh mục từ Web CMS xuống App
            string[] names = { "Tất cả", "Ốc & Hải sản", "Đồ nướng", "Đồ uống", "Nhà hàng", "Ăn vặt", "Khác" };
            string[] icons = { "🔥", "🐚", "🍖", "🍹", "🍽️", "🍡", "✨" };

            for (int i = 0; i < names.Length; i++)
            {
                bool isSelected = i == 0;
                var btn = CreateCategoryButton(names[i], icons[i], isSelected);

                var tapGesture = new TapGestureRecognizer();
                string categoryName = names[i];
                tapGesture.Tapped += (s, e) => OnCategorySelected((Border)s, categoryName);
                btn.GestureRecognizers.Add(tapGesture);

                _categoryButtons.Add(btn);
                stack.Children.Add(btn);
            }

            return new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = stack, HorizontalScrollBarVisibility = ScrollBarVisibility.Never };
        }

        private void OnCategorySelected(Border selectedBtn, string categoryName)
        {
            foreach (var btn in _categoryButtons)
            {
                bool isSelected = (btn == selectedBtn);

                btn.BackgroundColor = isSelected ? Color.FromArgb("#FF5C0F") : Colors.White;
                btn.Stroke = isSelected ? Colors.Transparent : Color.FromArgb("#E5E5E5");
                btn.StrokeThickness = isSelected ? 0 : 1;
                btn.Shadow = isSelected ? new Shadow { Brush = Color.FromArgb("#FF5C0F"), Opacity = 0.3f, Offset = new Point(0, 4), Radius = 8 } : null;

                var layout = (HorizontalStackLayout)btn.Content;
                var label = (Label)layout.Children[1];
                label.TextColor = isSelected ? Colors.White : Color.FromArgb("#4A4A4A");
                label.FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None;
            }

            if (BindingContext is HomeViewModel vm)
            {
                vm.FilterByCategory(categoryName);
            }
        }

        private Border CreateCategoryButton(string text, string icon, bool isSelected = false)
        {
            var layout = new HorizontalStackLayout { Spacing = 6, Padding = new Thickness(16, 8) };

            layout.Children.Add(new Label { Text = icon, FontSize = 16, VerticalOptions = LayoutOptions.Center });

            var textLabel = new Label
            {
                TextColor = isSelected ? Colors.White : Color.FromArgb("#4A4A4A"),
                FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 14
            };
            textLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: text));
            layout.Children.Add(textLabel);

            return new Border
            {
                Stroke = isSelected ? Colors.Transparent : Color.FromArgb("#E5E5E5"),
                StrokeThickness = isSelected ? 0 : 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                BackgroundColor = isSelected ? Color.FromArgb("#FF5C0F") : Colors.White,
                Content = layout,
                Shadow = isSelected ? new Shadow { Brush = Color.FromArgb("#FF5C0F"), Opacity = 0.3f, Offset = new Point(0, 4), Radius = 8 } : null
            };
        }

        // ========================================================
        // TẠO TAB BAR BẰNG CODE
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

            var tab1 = CreateTabItem("Trang chủ", "🏠", true);
            var tapHome = new TapGestureRecognizer();
            tab1.GestureRecognizers.Add(tapHome);
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "🗺️", false);
            var tapMap = new TapGestureRecognizer();
            tapMap.Tapped += async (s, e) => await Navigation.PushAsync(new MapPage(), false);
            tab2.GestureRecognizers.Add(tapMap);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "⚙️", false);
            var tapSettings = new TapGestureRecognizer();
            tapSettings.Tapped += async (s, e) => await Navigation.PushAsync(new SettingsPage(), false);
            tab3.GestureRecognizers.Add(tapSettings);
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
                source: VinhKhanhTour.Services.LocalizationResourceManager.Instance,
                converter: VinhKhanhTour.Helpers.TranslateConverter.Instance,
                converterParameter: text
            ));
            layout.Children.Add(textLabel);

            return layout;
        }
    }
}