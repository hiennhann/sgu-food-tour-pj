using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel; // Để dùng tính năng mở Map của thiết bị
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VinhKhanhTour.Helpers;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Utilities;

namespace VinhKhanhTour.Views
{
    [QueryProperty(nameof(SelectedPoiId), "poiId")]
    public partial class MapPage : ContentPage
    {
        private bool _isTrackingLocation = false;
        private int _selectedPoiId = 0;

        public int SelectedPoiId
        {
            get => _selectedPoiId;
            set => _selectedPoiId = value;
        }

        private int _playingPoiId = 0;
        private int _currentDwellingPoiId = 0;
        private bool _isPlaying = true;

        private Poi _nearestPoi; // Biến lưu trữ quán gần nhất để chỉ đường

        private IDispatcherTimer? _locationTimer;
        private Location? _currentUserLocation;

        public MapPage()
        {
            InitializeComponent();

            var tabBarBorder = CreateTabBar();
            Grid.SetRow(tabBarBorder, 1);
            RootGrid.Children.Add(tabBarBorder);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Task.Delay(500).ContinueWith(async (t) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await StartLocationTracking();
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopLocationTracking();
            NarrationEngine.Instance.CancelCurrentNarration();
        }

        // --- GEOFENCING LOGIC (Giữ nguyên, hoàn toàn không tốn phí) ---

        private async Task StartLocationTracking()
        {
            if (_isTrackingLocation) return;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    _isTrackingLocation = true;
                    _locationTimer = Dispatcher.CreateTimer();
                    _locationTimer.Interval = TimeSpan.FromSeconds(5);
                    _locationTimer.Tick += async (s, e) => await CheckLocation();
                    _locationTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi bật GPS: {ex.Message}");
            }
        }

        private void StopLocationTracking()
        {
            if (!_isTrackingLocation) return;
            _isTrackingLocation = false;
            if (_locationTimer != null)
            {
                _locationTimer.Stop();
                _locationTimer = null;
            }
        }

        private async Task CheckLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
                Location? location = await Geolocation.Default.GetLocationAsync(request);
                if (location == null) location = await Geolocation.Default.GetLastKnownLocationAsync();
                if (location != null) Geolocation_LocationChanged(this, new GeolocationLocationChangedEventArgs(location));
            }
            catch { }
        }

        private void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            _currentUserLocation = e.Location;

            Task.Run(async () =>
            {
                var pois = Poi.GetSampleData();
                Poi nearestTriggeredPoi = null;
                double minDistance = double.MaxValue;
                double nearestDistance = double.MaxValue;
                Poi closestPoi = null; // Quán gần nhất (dù chưa bước vào bán kính)

                foreach (var poi in pois)
                {
                    double distanceToPoi = LocationHelper.CalculateDistanceInMeters(_currentUserLocation.Latitude, _currentUserLocation.Longitude, poi.Latitude, poi.Longitude);
                    if (distanceToPoi < nearestDistance)
                    {
                        nearestDistance = distanceToPoi;
                        closestPoi = poi;
                    }

                    double effectiveRadius = (_currentDwellingPoiId == poi.Id) ? (poi.Radius + 10) : poi.Radius;
                    if (distanceToPoi <= effectiveRadius)
                    {
                        if (distanceToPoi < minDistance || (distanceToPoi == minDistance && (nearestTriggeredPoi == null || poi.Priority > nearestTriggeredPoi.Priority)))
                        {
                            nearestTriggeredPoi = poi;
                            minDistance = distanceToPoi;
                        }
                    }
                }

                _nearestPoi = closestPoi; // Lưu lại để nút "Chỉ đường" sử dụng

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (DebugLabel != null) DebugLabel.Text = $"GPS: {_currentUserLocation.Latitude:F5}, {_currentUserLocation.Longitude:F5}";
                    if (DistanceLabel != null) DistanceLabel.Text = $"{Services.LocalizationResourceManager.Instance["Cách quán gần nhất"] ?? "Cách quán gần nhất"}: {nearestDistance:F0}m";
                    if (PoiNameLabel != null) PoiNameLabel.Text = nearestTriggeredPoi != null ? nearestTriggeredPoi.Name : (closestPoi?.Name ?? "Chưa tìm thấy");

                    if (nearestTriggeredPoi != null)
                    {
                        if (_currentDwellingPoiId != nearestTriggeredPoi.Id)
                        {
                            _currentDwellingPoiId = nearestTriggeredPoi.Id;
                            if (_isPlaying)
                            {
                                _playingPoiId = nearestTriggeredPoi.Id;
                                await NarrationEngine.Instance.PlayNarrationAsync(ConvertPoiToFoodPlace(nearestTriggeredPoi), isManual: false);
                            }
                        }
                    }
                    else
                    {
                        if (_currentDwellingPoiId != 0)
                        {
                            _currentDwellingPoiId = 0;
                            _playingPoiId = 0;
                        }
                    }
                });
            });
        }

        private FoodPlace ConvertPoiToFoodPlace(Poi p)
        {
            return new FoodPlace { Id = p.Id.ToString(), Name = p.Name, NarrationText = p.DisplayTtsScript };
        }

        // ========================================================
        // TÍNH NĂNG CHỈ ĐƯỜNG (GỌI GOOGLE MAPS CỦA ĐIỆN THOẠI)
        // ========================================================
        private async void OnRouteClicked(object sender, EventArgs e)
        {
            Poi target = null;

            // Nếu đi từ trang chủ vào (có ID), thì ưu tiên chỉ đường đến ID đó
            if (_selectedPoiId > 0)
            {
                target = Poi.GetSampleData().FirstOrDefault(p => p.Id == _selectedPoiId);
            }

            // Nếu không, chỉ đường đến quán ăn gần nhất mà Radar đang quét thấy
            if (target == null)
            {
                target = _nearestPoi;
            }

            if (target != null)
            {
                var location = new Location(target.Latitude, target.Longitude);
                var options = new MapLaunchOptions
                {
                    Name = target.Name,
                    NavigationMode = NavigationMode.Driving // Tự động bật chế độ lái xe/chỉ đường
                };

                try
                {
                    // Lệnh này sẽ mở thẳng ứng dụng Google Maps (trên Android) 
                    // hoặc Apple Maps (trên iOS) mà không tốn một đồng API nào!
                    await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Lỗi", "Không thể mở ứng dụng bản đồ trên máy của bạn.", "OK");
                }
            }
        }

        private void OnPlayPauseClicked(object sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            MainThread.BeginInvokeOnMainThread(() => { if (PlayPauseButton != null) PlayPauseButton.Text = _isPlaying ? "⏸" : "▶"; });
            if (!_isPlaying) NarrationEngine.Instance.CancelCurrentNarration();
        }

        // --- TẠO TAB BAR BẰNG CODE ---
        private Border CreateTabBar()
        {
            var tabBarGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } }, BackgroundColor = Colors.White, Padding = new Thickness(0, 5, 0, 5) };

            var tab1 = CreateTabItem("Trang chủ", "icon_home.png", false);
            var tapHome = new TapGestureRecognizer(); tapHome.Tapped += async (s, e) => await Navigation.PushAsync(new HomePage(), false); tab1.GestureRecognizers.Add(tapHome);
            Grid.SetColumn(tab1, 0); tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "icon_map.png", true);
            Grid.SetColumn(tab2, 1); tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "icon_settings.png", false);
            var tapSettings = new TapGestureRecognizer(); tapSettings.Tapped += async (s, e) => await Navigation.PushAsync(new SettingsPage(), false); tab3.GestureRecognizers.Add(tapSettings);
            Grid.SetColumn(tab3, 2); tabBarGrid.Children.Add(tab3);

            return new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(15, 15, 0, 0) }, BackgroundColor = Colors.White, Content = tabBarGrid, Stroke = Color.FromArgb("#E0E0E0") };
        }

        private View CreateTabItem(string text, string icon, bool isSelected = false)
        {
            var layout = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            layout.Children.Add(new Image { Source = icon, HeightRequest = 24, WidthRequest = 24, Opacity = isSelected ? 1.0 : 0.5 });
            var textLabel = new Label { TextColor = isSelected ? Color.FromArgb("#FF5C0F") : Color.FromArgb("#808080"), FontSize = 10, HorizontalOptions = LayoutOptions.Center };
            textLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: LocalizationResourceManager.Instance, converter: TranslateConverter.Instance, converterParameter: text));
            layout.Children.Add(textLabel); 
            return layout;
        }
    }
}