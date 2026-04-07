using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Devices;
using System.Collections.Generic;
using VinhKhanhTour.Models;
using VinhKhanhTour.Data;
using VinhKhanhTour.Services; // Thêm thư viện này
using VinhKhanhTour.Helpers;  // Thêm thư viện này

namespace VinhKhanhTour.Views
{
    public class MenuPage : ContentPage
    {
        public MenuPage(FoodPlace place)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#F4F4F6");

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Star } }
            };

            // ==========================================
            // 1. HEADER (Đã gắn Binding đa ngôn ngữ)
            // ==========================================
            var headerGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 60 }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = 60 } },
                Padding = new Thickness(10, DeviceInfo.Platform == DevicePlatform.iOS ? 50 : 20, 10, 15),
                BackgroundColor = Colors.White
            };

            var backBtn = new Button { Text = "❮", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FF5C0F"), BackgroundColor = Color.FromArgb("#FFF0ED"), WidthRequest = 40, HeightRequest = 40, CornerRadius = 20, Padding = 0 };
            backBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            Grid.SetColumn(backBtn, 0);
            headerGrid.Children.Add(backBtn);

            var titleStack = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            // ĐÃ FIX BINDING: Chữ "Thực Đơn"
            var menuLabel = new Label { FontSize = 12, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center };
            menuLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "Thực Đơn"));
            titleStack.Children.Add(menuLabel);

            titleStack.Children.Add(new Label { Text = place.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, MaxLines = 1, LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Center });

            Grid.SetColumn(titleStack, 1);
            headerGrid.Children.Add(titleStack);

            var headerBorder = new Border { Content = headerGrid, StrokeThickness = 0, Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.05f, Offset = new Point(0, 4), Radius = 8 } };
            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // ==========================================
            // 2. DANH SÁCH MÓN ĂN
            // ==========================================
            var scrollContent = new ScrollView();
            var listStack = new VerticalStackLayout { Spacing = 15, Padding = new Thickness(20, 15, 20, 40) };

            var noteBorder = new Border { BackgroundColor = Color.FromArgb("#E3F2FD"), StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 }, Padding = new Thickness(15, 10), Margin = new Thickness(0, 0, 0, 10) };

            // ĐÃ FIX BINDING: Dòng chữ Ghi chú
            var noteLabel = new Label { FontSize = 13, TextColor = Color.FromArgb("#1565C0"), FontAttributes = FontAttributes.Italic, HorizontalTextAlignment = TextAlignment.Center };
            noteLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "💡 Giá cả chỉ mang tính chất tham khảo, có thể thay đổi tùy thời điểm."));
            noteBorder.Content = noteLabel;
            listStack.Children.Add(noteBorder);

            List<FoodItem> items = MenuDataStore.GetMenuForPlace(place.Name);

            if (items == null || items.Count == 0)
            {
                // ĐÃ FIX BINDING: Dòng chữ Đang cập nhật
                var emptyLabel = new Label { TextColor = Colors.Gray, FontSize = 16, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 50, 0, 0) };
                emptyLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: "🍽️ Đang cập nhật thực đơn..."));
                listStack.Children.Add(emptyLabel);
            }
            else
            {
                foreach (var item in items) listStack.Children.Add(CreateFoodItem(item.Name, item.Price, item.ImageUrl));
            }

            scrollContent.Content = listStack;
            Grid.SetRow(scrollContent, 1);
            mainGrid.Children.Add(scrollContent);

            Content = mainGrid;
        }

        private Border CreateFoodItem(string name, string price, string imageSrc)
        {
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 85 }, new ColumnDefinition { Width = GridLength.Star } }, ColumnSpacing = 15 };
            var imageBorder = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }, Content = new Image { Source = imageSrc, Aspect = Aspect.AspectFill }, HeightRequest = 85, WidthRequest = 85, Stroke = Colors.Transparent, BackgroundColor = Color.FromArgb("#EAEAEA") };
            Grid.SetColumn(imageBorder, 0); grid.Children.Add(imageBorder);

            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 6 };
            infoStack.Children.Add(new Label { Text = name, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation });
            infoStack.Children.Add(new Label { Text = price, FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FF5C0F") });
            Grid.SetColumn(infoStack, 1); grid.Children.Add(infoStack);

            return new Border { BackgroundColor = Colors.White, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 }, Padding = new Thickness(10), Stroke = Colors.Transparent, Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.06f, Offset = new Point(0, 5), Radius = 12 }, Content = grid };
        }
    }
}