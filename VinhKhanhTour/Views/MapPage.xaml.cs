using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
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
        private bool _isMapLoaded = false;
        private int _selectedPoiId = 0;

        public int SelectedPoiId
        {
            get => _selectedPoiId;
            set => _selectedPoiId = value;
        }

        private int _playingPoiId = 0;
        private int _currentDwellingPoiId = 0;
        private bool _isPlaying = true;
        private List<Poi> _cachedPois = new();

        private Poi _nearestPoi;

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
            if (!_isMapLoaded)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadPoisToMapAsync();
                    _isMapLoaded = true;
                });
            }

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

        // ========================================================
        // HTML BẢN ĐỒ: THÊM SỰ KIỆN CLICK CHO MAUI
        // ========================================================
        private static string TaoHtmlBanDo(List<Poi> danhSach, int selectedId)
        {
            var jsGhim = new System.Text.StringBuilder();
            foreach (var q in danhSach)
            {
                var tenEscaped = q.Name?.Replace("'", "\\'").Replace("\"", "\\\"").Replace("\n", " ");
                jsGhim.AppendLine(
                    $"themGhim({q.Id}, \"{tenEscaped}\", " +
                    $"{q.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                    $"{q.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                    $"{q.Radius.ToString(System.Globalization.CultureInfo.InvariantCulture)});"
                );
            }

            double centerLat = 10.761622;
            double centerLng = 106.661172;

            if (selectedId > 0)
            {
                var selectedPoi = danhSach.FirstOrDefault(p => p.Id == selectedId);
                if (selectedPoi != null)
                {
                    centerLat = selectedPoi.Latitude;
                    centerLng = selectedPoi.Longitude;
                }
            }

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
    <style>
        * {{ margin:0; padding:0; box-sizing:border-box; }}
        body {{ background:#f0ede8; }}
        #map {{ width:100vw; height:100vh; }}
        .marker-pin {{ width: 32px; height: 32px; border-radius: 50% 50% 50% 0; background: #2D6A4F; transform: rotate(-45deg); border: 3px solid white; box-shadow: 0 2px 8px rgba(0,0,0,0.3); transition: all 0.3s ease; }}
        .marker-pin::after {{ content: ''; width: 12px; height: 12px; background: white; border-radius: 50%; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); }}
        .marker-pin.selected {{ background: #FF5C0F; width: 40px; height: 40px; box-shadow: 0 4px 16px rgba(255,92,15,0.5); }}
        .marker-pin.playing {{ background: #FFD700; width: 38px; height: 38px; box-shadow: 0 4px 16px rgba(255,215,0,0.6); animation: pulse-yellow 2s infinite; }}
        @keyframes pulse-yellow {{ 0% {{ box-shadow: 0 0 0 0 rgba(255,215,0,0.6); }} 70% {{ box-shadow: 0 0 0 14px rgba(255,215,0,0); }} 100% {{ box-shadow: 0 0 0 0 rgba(255,215,0,0); }} }}
        .marker-wrapper {{ width: 40px; height: 50px; }}
        .user-loc {{ width: 22px; height: 22px; background: #1A73E8; border: 4px solid white; border-radius: 50%; box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); animation: u-pulse 2s infinite; }}
        @keyframes u-pulse {{ 0% {{ box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); }} 50% {{ box-shadow: 0 0 0 20px rgba(26,115,232,0.08), 0 2px 8px rgba(0,0,0,0.35); }} 100% {{ box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); }} }}
        .user-loc-wrap {{ width: 30px; height: 30px; }}
        .leaflet-popup-content-wrapper {{ border-radius: 14px; box-shadow: 0 4px 20px rgba(0,0,0,0.15); border: none; }}
        .leaflet-popup-content {{ margin: 12px 16px; font-family: -apple-system, sans-serif; text-align: center; }}
        .popup-ten {{ font-weight: 700; font-size: 14px; color: #1A1A1A; margin-bottom: 2px; }}
        .leaflet-control-zoom {{ display: none; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <script>
        var map = L.map('map', {{ zoomControl: false, attributionControl: false }}).setView([{centerLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {centerLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}], 17);
        L.tileLayer('https://{{s}}.basemaps.cartocdn.com/light_all/{{z}}/{{x}}/{{y}}{{r}}.png', {{ subdomains: 'abcd', maxZoom: 20 }}).addTo(map);

        var allMarkers = {{}};
        var allCircles = {{}};
        var userMarker = null;

        function taoIcon(state) {{
            var cls = 'marker-pin';
            if (state === 'selected') cls += ' selected';
            else if (state === 'playing') cls += ' playing';
            return L.divIcon({{ className: 'marker-wrapper', html: ""<div class='"" + cls + ""'></div>"", iconSize: [40, 50], iconAnchor: [20, 50], popupAnchor: [0, -54] }});
        }}

        function themGhim(id, ten, lat, lng, radius) {{
            var circle = L.circle([lat, lng], {{ color: '#A9A9A9', fillColor: '#A9A9A9', fillOpacity: 0.2, radius: radius, weight: 2 }}).addTo(map);
            allCircles[id] = circle;

            var marker = L.marker([lat, lng], {{ icon: taoIcon('normal') }}).addTo(map);
            marker.bindPopup(""<div class='popup-ten'>"" + ten + ""</div>"");
            
            // 🔴 JS BẮN TÍN HIỆU CLICK GHIM SANG C#
            marker.on('click', function() {{
                window.location.href = 'tappin://' + id;
            }});

            allMarkers[id] = marker;
        }}

        // 🔴 JS BẮN TÍN HIỆU CLICK RA NGOÀI ĐỂ HỦY CHỌN
        map.on('click', function() {{
            window.location.href = 'mapclick://clear';
        }});

        function updateMarkerState(id, state) {{
            for (let key in allMarkers) {{
                allMarkers[key].setIcon(taoIcon('normal'));
                allCircles[key].setStyle({{ color: '#A9A9A9', fillColor: '#A9A9A9' }});
            }}
            if (id && allMarkers[id]) {{
                allMarkers[id].setIcon(taoIcon(state));
                if(state === 'playing') {{
                    allCircles[id].setStyle({{ color: '#FFD700', fillColor: '#FFD700', fillOpacity: 0.4 }});
                }} else if (state === 'selected') {{
                    allCircles[id].setStyle({{ color: '#FF5C0F', fillColor: '#FF5C0F', fillOpacity: 0.3 }});
                }}
            }}
        }}

        function setUserLocation(lat, lng) {{
            if (userMarker) {{ userMarker.setLatLng([lat, lng]); }} 
            else {{
                var icon = L.divIcon({{ className: 'user-loc-wrap', html: ""<div class='user-loc'></div>"", iconSize: [24, 24], iconAnchor: [12, 12] }});
                userMarker = L.marker([lat, lng], {{ icon: icon, zIndexOffset: 1000 }}).addTo(map);
            }}
        }}

        {jsGhim}
        updateMarkerState({selectedId}, 'selected');
    </script>
</body>
</html>";
        }

        private async Task LoadPoisToMapAsync()
        {
            try
            {
                var poiRepo = MauiProgram.Services.GetService<Data.PoiRepository>();
                if (poiRepo != null) _cachedPois = await poiRepo.GetAllPoisAsync();
                if (_cachedPois == null || !_cachedPois.Any()) _cachedPois = Poi.GetSampleData();

                var html = TaoHtmlBanDo(_cachedPois, _selectedPoiId);
                BanDoWebView.Source = new HtmlWebViewSource { Html = html };
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        // ========================================================
        // NHẬN SỰ KIỆN TƯƠNG TÁC TỪ JAVASCRIPT
        // ========================================================
        // ========================================================
        // NHẬN SỰ KIỆN TƯƠNG TÁC TỪ JAVASCRIPT (ĐÃ FIX LỖI DẤU / )
        // ========================================================
        private void BanDoWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            string url = e.Url.ToLower();

            // 1. Nếu người dùng chọn vào 1 Ghim
            if (url.StartsWith("tappin://"))
            {
                e.Cancel = true; // Ngăn không cho load trang web mới

                // LẤY ID VÀ XÓA DẤU '/' Ở CUỐI (Fix lỗi Android WebView)
                string idStr = url.Replace("tappin://", "").TrimEnd('/');

                if (int.TryParse(idStr, out int idQuan))
                {
                    _selectedPoiId = idQuan;
                    var selectedPoi = _cachedPois.FirstOrDefault(p => p.Id == idQuan);

                    if (selectedPoi != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            CardStatusLabel.Text = "ĐỊA ĐIỂM ĐÃ CHỌN";
                            PoiNameLabel.Text = selectedPoi.Name;
                            DistanceLabel.Text = "👆 Chạm vào thẻ này để xem Menu";

                            // Đổi màu ghim trên JS thành Cam (Selected)
                            await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({_selectedPoiId}, 'selected');");
                        });
                    }
                }
            }
            // 2. Nếu người dùng click vào vùng trống trên map (Hủy chọn)
            else if (url.StartsWith("mapclick://"))
            {
                e.Cancel = true;
                _selectedPoiId = 0; // Trả về trạng thái Radar tự động quét

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    CardStatusLabel.Text = "BẠN ĐANG Ở GẦN";
                    PoiNameLabel.Text = _nearestPoi?.Name ?? "Đang dò tìm...";
                    DistanceLabel.Text = "Đang theo dõi Radar GPS...";

                    // Reset bản đồ về trạng thái bình thường (normal)
                    await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState(0, 'normal');");
                });
            }
        }

        // ========================================================
        // CLICK VÀO THẺ ĐỂ MỞ TRANG CHI TIẾT (PlaceDetailPage)
        // ========================================================
        private async void BottomInfoCard_Tapped(object sender, EventArgs e)
        {
            int targetId = _selectedPoiId > 0 ? _selectedPoiId : (_nearestPoi?.Id ?? 0);

            if (targetId > 0)
            {
                var poi = _cachedPois.FirstOrDefault(p => p.Id == targetId);
                if (poi != null)
                {
                    // Truyền dữ liệu sang trang PlaceDetailPage
                    var foodPlace = ConvertPoiToFoodPlace(poi);
                    await Navigation.PushAsync(new PlaceDetailPage(foodPlace));
                }
            }
            else
            {
                // Thông báo cho người dùng nếu chưa có dữ liệu quán
                await DisplayAlert("Thông báo", "Vui lòng chạm chọn một quán trên bản đồ, hoặc đợi Radar quét vị trí của bạn.", "OK");
            }
        }

        private async Task StartLocationTracking()
        {
            if (_isTrackingLocation) return;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    _isTrackingLocation = true;
                    _locationTimer = Dispatcher.CreateTimer();
                    _locationTimer.Interval = TimeSpan.FromSeconds(5);
                    _locationTimer.Tick += async (s, e) => await CheckLocation();
                    _locationTimer.Start();
                }
            }
            catch { }
        }

        private void StopLocationTracking()
        {
            if (!_isTrackingLocation) return;
            _isTrackingLocation = false;
            if (_locationTimer != null) { _locationTimer.Stop(); _locationTimer = null; }
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
                var pois = _cachedPois.Any() ? _cachedPois : Poi.GetSampleData();
                Poi nearestTriggeredPoi = null;
                double minDistance = double.MaxValue;
                double nearestDistance = double.MaxValue;
                Poi closestPoi = null;

                foreach (var poi in pois)
                {
                    double distanceToPoi = LocationHelper.CalculateDistanceInMeters(_currentUserLocation.Latitude, _currentUserLocation.Longitude, poi.Latitude, poi.Longitude);
                    if (distanceToPoi < nearestDistance) { nearestDistance = distanceToPoi; closestPoi = poi; }

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

                _nearestPoi = closestPoi;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var latStr = _currentUserLocation.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var lngStr = _currentUserLocation.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    await BanDoWebView.EvaluateJavaScriptAsync($"setUserLocation({latStr}, {lngStr});");

                    if (DebugLabel != null) DebugLabel.Text = $"GPS: {_currentUserLocation.Latitude:F5}, {_currentUserLocation.Longitude:F5}";

                    // CHỈ cập nhật nhãn nếu người dùng KHÔNG bấm chọn quán thủ công
                    if (_selectedPoiId == 0)
                    {
                        CardStatusLabel.Text = "BẠN ĐANG Ở GẦN";
                        if (DistanceLabel != null) DistanceLabel.Text = $"{Services.LocalizationResourceManager.Instance["Cách quán gần nhất"] ?? "Cách quán gần nhất"}: {nearestDistance:F0}m";
                        if (PoiNameLabel != null) PoiNameLabel.Text = nearestTriggeredPoi != null ? nearestTriggeredPoi.Name : (closestPoi?.Name ?? "Chưa tìm thấy");
                    }

                    if (nearestTriggeredPoi != null)
                    {
                        if (_currentDwellingPoiId != nearestTriggeredPoi.Id)
                        {
                            _currentDwellingPoiId = nearestTriggeredPoi.Id;
                            if (_isPlaying)
                            {
                                _playingPoiId = nearestTriggeredPoi.Id;
                                await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({_playingPoiId}, 'playing');");
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
                            await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({_selectedPoiId}, 'selected');");
                        }
                    }
                });
            });
        }

        private FoodPlace ConvertPoiToFoodPlace(Poi p)
        {
            // Truyền ImageUrl sang để PlaceDetailPage có ảnh
            return new FoodPlace { Id = p.Id.ToString(), Name = p.Name, NarrationText = p.DisplayTtsScript, ImageUrl = p.ImageUrl };
        }

        // ========================================================
        // TÍNH NĂNG CHỈ ĐƯỜNG MỞ GOOGLE MAPS
        // ========================================================
        private async void OnRouteClicked(object sender, EventArgs e)
        {
            // Lấy ID đang được thao tác (ưu tiên click thủ công, nếu không thì lấy quán gần nhất)
            Poi target = (_selectedPoiId > 0) ? _cachedPois.FirstOrDefault(p => p.Id == _selectedPoiId) : _nearestPoi;

            if (target != null)
            {
                var location = new Location(target.Latitude, target.Longitude);
                var options = new MapLaunchOptions
                {
                    Name = target.Name,
                    NavigationMode = NavigationMode.Driving
                };

                try { await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options); }
                catch (Exception ex) { await DisplayAlert("Lỗi", "Không thể mở ứng dụng bản đồ.", "OK"); }
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