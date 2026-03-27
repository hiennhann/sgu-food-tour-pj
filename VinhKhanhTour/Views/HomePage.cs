using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using VinhKhanhTour.Views;
using VinhKhanhTour.ViewModels; // Thêm dòng này để gọi ViewModel
using VinhKhanhTour.Models;     // Thêm dòng này để gọi Model

namespace VinhKhanhTour
{
    public class HomePage : ContentPage
    {
        public HomePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Colors.White;

            // 1. KẾT NỐI VIEW VÀ VIEWMODEL TẠI ĐÂY (Phép thuật bắt đầu)
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

            // --- HEADER (Giữ nguyên) ---
            var headerGrid = new Grid();
            headerGrid.Children.Add(new Image { Source = "hero_vinh_khanh.jpg", Aspect = Aspect.AspectFill, HeightRequest = 230 });
            headerGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#40000000") });

            var playBtn = new ImageButton { Source = "icon_play.png", BackgroundColor = Color.FromArgb("#FF5C0F"), WidthRequest = 60, HeightRequest = 60, CornerRadius = 30, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            headerGrid.Children.Add(playBtn);

            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);

            // --- NỘI DUNG CUỘN ---
            var contentScrollView = new ScrollView();
            var contentStack = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(15, 15, 15, 0) };

            var searchBarGrid = new Grid();
            var searchEntry = new Entry { Placeholder = "🔍 Tìm kiếm món ăn, quán...", BackgroundColor = Color.FromArgb("#F2F2F2"), HeightRequest = 45 };
            searchBarGrid.Children.Add(new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Content = searchEntry, Stroke = Colors.Transparent });
            contentStack.Children.Add(searchBarGrid);

            contentStack.Children.Add(CreateCategoryList());

            var ctaButton = new Button { Text = "🗺️ Bắt Đầu Tour Ngay →", BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 25, HeightRequest = 50, Margin = new Thickness(0, 10) };
            ctaButton.Clicked += async (s, e) => await Navigation.PushAsync(new MapPage());
            contentStack.Children.Add(ctaButton);

            var featuredHeader = new HorizontalStackLayout { Spacing = 5 };
            featuredHeader.Children.Add(new Label { Text = "📍", TextColor = Color.FromArgb("#FF5C0F"), FontSize = 20 });
            featuredHeader.Children.Add(new Label { Text = "Địa điểm nổi bật", FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black, VerticalOptions = LayoutOptions.Center });
            contentStack.Children.Add(featuredHeader);

            // =========================================================
            // 2. VẼ DANH SÁCH BẰNG COLLECTION VIEW (THAY THẾ CODE CŨ)
            // =========================================================
            var placesCollectionView = new CollectionView();

            // Trói buộc dữ liệu vào danh sách FeaturedPlaces trong ViewModel
            placesCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "FeaturedPlaces");

            // Định nghĩa Khuôn mẫu (Template) cho MỘT thẻ quán ăn
            placesCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var cardGrid = new Grid { HeightRequest = 180, Margin = new Thickness(0, 0, 0, 15) };

                // Trói buộc Ảnh
                var bgImage = new Image { Aspect = Aspect.AspectFill };
                bgImage.SetBinding(Image.SourceProperty, "ImageUrl");
                cardGrid.Children.Add(new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 }, Content = bgImage, Stroke = Colors.Transparent });

                cardGrid.Children.Add(new BoxView { BackgroundColor = Color.FromArgb("#30000000") });

                // Trói buộc Tên quán
                var titleLabel = new Label { TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.End, Margin = new Thickness(15, 0, 0, 15) };
                titleLabel.SetBinding(Label.TextProperty, "Name");
                cardGrid.Children.Add(titleLabel);

                var infoGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Star } }, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End, Margin = new Thickness(0, 0, 15, 15), ColumnSpacing = 5 };

                // Trói buộc Đánh giá (Rating + Số lượt)
                var ratingStack = new HorizontalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 3 };
                ratingStack.Children.Add(new Label { Text = "⭐", TextColor = Colors.White, FontSize = 12 });
                var rateVal = new Label { TextColor = Colors.White, FontSize = 12 };
                rateVal.SetBinding(Label.TextProperty, "Rating");
                ratingStack.Children.Add(rateVal);
                var revCount = new Label { TextColor = Colors.White, FontSize = 12 };
                revCount.SetBinding(Label.TextProperty, new Binding("ReviewCount", stringFormat: "({0})"));
                ratingStack.Children.Add(revCount);

                Grid.SetColumn(ratingStack, 0);
                infoGrid.Children.Add(ratingStack);

                var playBtn = new Button { Text = "▶️ Phát Audio", BackgroundColor = Color.FromArgb("#FF5C0F"), TextColor = Colors.White, FontSize = 10, Padding = new Thickness(10, 5), CornerRadius = 10, HeightRequest = 30 };
                Grid.SetColumn(playBtn, 1);
                infoGrid.Children.Add(playBtn);
                cardGrid.Children.Add(infoGrid);

                var headphoneIcon = new Image { Source = "icon_headphone.png", HeightRequest = 25, WidthRequest = 25, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0, 15, 15, 0) };
                cardGrid.Children.Add(headphoneIcon);

                // Cài đặt nút bấm chuyển trang
                // Cài đặt nút bấm chuyển trang có truyền dữ liệu
                var tapPlace = new TapGestureRecognizer();
                tapPlace.Tapped += async (s, e) =>
                {
                    // Lấy ra đúng cái quán ăn (FoodPlace) mà người dùng vừa bấm vào
                    var selectedPlace = (FoodPlace)((BindableObject)s).BindingContext;

                    // Xách dữ liệu đó truyền sang trang Chi tiết
                    await Navigation.PushAsync(new PlaceDetailPage(selectedPlace));
                };
                cardGrid.GestureRecognizers.Add(tapPlace);

                return cardGrid;
            });

            contentStack.Children.Add(placesCollectionView);
            // =========================================================

            contentScrollView.Content = contentStack;
            Grid.SetRow(contentScrollView, 1);
            mainGrid.Children.Add(contentScrollView);

            // --- TAB BAR (Giữ nguyên) ---
            var tabBarGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, BackgroundColor = Colors.White, Padding = new Thickness(0, 5, 0, 5) };

            var tab1 = CreateTabItem("Trang chủ", "icon_home.png", true);
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "icon_map.png", false);
            var tapMap = new TapGestureRecognizer();
            tapMap.Tapped += async (s, e) => await Navigation.PushAsync(new MapPage(), false);
            tab2.GestureRecognizers.Add(tapMap);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "icon_settings.png", false);
            var tapSettings = new TapGestureRecognizer();
            tapSettings.Tapped += async (s, e) => await Navigation.PushAsync(new SettingsPage(), false);
            tab3.GestureRecognizers.Add(tapSettings);
            Grid.SetColumn(tab3, 2);
            tabBarGrid.Children.Add(tab3);

            var tabBarBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(15, 15, 0, 0) }, BackgroundColor = Colors.White, Content = tabBarGrid, Stroke = Colors.Transparent };
            Grid.SetRow(tabBarBorder, 2);
            mainGrid.Children.Add(tabBarBorder);

            Content = mainGrid;
        }

        private View CreateCategoryList()
        {
            var stack = new HorizontalStackLayout { Spacing = 10, Padding = new Thickness(0, 0) };
            stack.Children.Add(CreateCategoryButton("Tất cả", "icon_fire.png", true));
            stack.Children.Add(CreateCategoryButton("Ốc & Hải sản", "icon_snail.png"));
            stack.Children.Add(CreateCategoryButton("Đồ nướng", "icon_skewer.png"));
            stack.Children.Add(CreateCategoryButton("Đồ uống", "icon_drink.png"));
            return new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = stack, HorizontalScrollBarVisibility = ScrollBarVisibility.Never };
        }

        private Border CreateCategoryButton(string text, string icon, bool isSelected = false)
        {
            var layout = new HorizontalStackLayout { Spacing = 5, Padding = new Thickness(15, 10) };
            layout.Children.Add(new Image { Source = icon, HeightRequest = 20, WidthRequest = 20 });
            layout.Children.Add(new Label { Text = text, TextColor = isSelected ? Colors.White : Colors.Black, VerticalOptions = LayoutOptions.Center });

            return new Border
            {
                Stroke = isSelected ? Colors.Transparent : Color.FromArgb("#E0E0E0"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
                BackgroundColor = isSelected ? Color.FromArgb("#FF5C0F") : Colors.White,
                Content = layout
            };
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