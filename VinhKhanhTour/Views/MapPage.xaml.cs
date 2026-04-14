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
        private bool _isPlaying = false;
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

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
    <style>
        * {{ margin:0; padding:0; box-sizing:border-box; }}
        html, body {{ width:100%; height:100%; overflow:hidden; }}
        body {{ background:#f0ede8; }}
        #map {{ position:absolute; top:0; bottom:0; left:0; right:0; }}
        .marker-pin {{ width: 32px; height: 32px; border-radius: 50% 50% 50% 0; background: #2D6A4F; transform: rotate(-45deg); border: 3px solid white; box-shadow: 0 2px 8px rgba(0,0,0,0.3); transition: all 0.3s ease; }}
        .marker-pin::after {{ content: ''; width: 12px; height: 12px; background: white; border-radius: 50%; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); }}
        .marker-pin.selected {{ background: #FF5C0F; width: 40px; height: 40px; box-shadow: 0 4px 16px rgba(255,92,15,0.5); z-index: 1000 !important; }}
        .marker-pin.playing {{ background: #FFD700; width: 38px; height: 38px; box-shadow: 0 4px 16px rgba(255,215,0,0.6); animation: pulse-yellow 2s infinite; }}
        @keyframes pulse-yellow {{ 0% {{ box-shadow: 0 0 0 0 rgba(255,215,0,0.6); }} 70% {{ box-shadow: 0 0 0 14px rgba(255,215,0,0); }} 100% {{ box-shadow: 0 0 0 0 rgba(255,215,0,0); }} }}
        .marker-wrapper {{ width: 40px; height: 50px; }}
        .user-loc {{ width: 22px; height: 22px; background: #1A73E8; border: 4px solid white; border-radius: 50%; box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); animation: u-pulse 2s infinite; }}
        @keyframes u-pulse {{ 0% {{ box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); }} 50% {{ box-shadow: 0 0 0 20px rgba(26,115,232,0.08), 0 2px 8px rgba(0,0,0,0.35); }} 100% {{ box-shadow: 0 0 0 8px rgba(26,115,232,0.30), 0 2px 8px rgba(0,0,0,0.35); }} }}
        .user-loc-wrap {{ width: 30px; height: 30px; z-index: 9999 !important; }}
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
        
        var isClickingMarker = false; 
        var currentSelectedId = 0;
        var currentPlayingId = 0;

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
            var popupHtml = `<div class='popup-ten' style='margin-bottom:8px;'>${{ten}}</div><button onclick='window.location.href=""openmap://${{lat}},${{lng}}"";' style='background-color:#FF5C0F; color:white; border:none; padding:6px 12px; border-radius:4px; font-weight:bold; cursor:pointer;'>📍 Mở GG Map</button>`;
            marker.bindPopup(popupHtml);
            
            marker.on('click', function(e) {{
                isClickingMarker = true;
                window.location.href = 'tappin://' + id;
                setTimeout(function() {{ isClickingMarker = false; }}, 500);
            }});

            allMarkers[id] = marker;
        }}

        map.on('click', function(e) {{
            if (isClickingMarker) return; 
            window.location.href = 'mapclick://clear';
        }});

        function updateMarkerState(id, state) {{
            if (state === 'selected') currentSelectedId = id;
            if (state === 'playing') currentPlayingId = id;
            if (state === 'normal' && id === 0) currentSelectedId = 0;

            for (let key in allMarkers) {{
                allMarkers[key].setIcon(taoIcon('normal'));
                allCircles[key].setStyle({{ color: '#A9A9A9', fillColor: '#A9A9A9', fillOpacity: 0.2 }});
            }}
            
            if (currentPlayingId > 0 && allMarkers[currentPlayingId]) {{
                allMarkers[currentPlayingId].setIcon(taoIcon('playing'));
                allCircles[currentPlayingId].setStyle({{ color: '#FFD700', fillColor: '#FFD700', fillOpacity: 0.4 }});
            }}

            if (currentSelectedId > 0 && allMarkers[currentSelectedId]) {{
                allMarkers[currentSelectedId].setIcon(taoIcon('selected'));
                allCircles[currentSelectedId].setStyle({{ color: '#FF5C0F', fillColor: '#FF5C0F', fillOpacity: 0.3 }});
            }}
        }}

        function setUserLocation(lat, lng) {{
            if (userMarker) {{ userMarker.setLatLng([lat, lng]); }} 
            else {{
                var icon = L.divIcon({{ className: 'user-loc-wrap', html: ""<div class='user-loc'></div>"", iconSize: [24, 24], iconAnchor: [12, 12] }});
                userMarker = L.marker([lat, lng], {{ icon: icon, zIndexOffset: 9999 }}).addTo(map);
            }}
        }}

        {jsGhim}
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

                // TUYỆT ĐỐI KHÔNG GỌI GetSampleData() NỮA
                // Nếu chưa có mạng hoặc API chưa trả về kịp, tạo một danh sách rỗng để không bị lỗi
                if (_cachedPois == null) _cachedPois = new List<Poi>();

                var html = TaoHtmlBanDo(_cachedPois, _selectedPoiId);
                BanDoWebView.Source = new HtmlWebViewSource { Html = html };
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void BanDoWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Url)) return;

            string url = e.Url.ToLower();

            if (url.StartsWith("tappin://"))
            {
                e.Cancel = true;
                string idStr = url.Replace("tappin://", "").TrimEnd('/');

                if (int.TryParse(idStr, out int idQuan))
                {
                    _selectedPoiId = idQuan;
                    var selectedPoi = _cachedPois.FirstOrDefault(p => p.Id == idQuan);

                    if (selectedPoi != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            CardStatusLabel.RemoveBinding(Label.TextProperty);
                            CardStatusLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "ĐỊA ĐIỂM ĐÃ CHỌN"));

                            PoiNameLabel.RemoveBinding(Label.TextProperty);
                            PoiNameLabel.Text = selectedPoi.Name;

                            DistanceLabel.RemoveBinding(Label.TextProperty);
                            DistanceLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Chọn Nghe Audio hoặc Chỉ đường"));

                            _isPlaying = false;
                            NarrationEngine.Instance.CancelCurrentNarration();
                            if (PlayPauseButton != null)
                            {
                                PlayPauseButton.Text = "▶️";
                                PlayPauseButton.BackgroundColor = Color.FromArgb("#FF5C0F");
                            }

                            try { await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({_selectedPoiId}, 'selected');"); }
                            catch { }
                        });
                    }
                }
            }
            else if (url.StartsWith("mapclick://"))
            {
                e.Cancel = true;
                _selectedPoiId = 0;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    CardStatusLabel.RemoveBinding(Label.TextProperty);
                    CardStatusLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "BẠN ĐANG Ở GẦN"));

                    PoiNameLabel.RemoveBinding(Label.TextProperty);
                    if (_nearestPoi != null)
                    {
                        PoiNameLabel.Text = _nearestPoi.Name;
                    }
                    else
                    {
                        PoiNameLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Đang dò tìm..."));
                    }

                    DistanceLabel.RemoveBinding(Label.TextProperty);
                    DistanceLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "Đang theo dõi Radar GPS..."));

                    try { await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState(0, 'normal');"); }
                    catch { }
                });
            }
            else if (url.StartsWith("openmap://"))
            {
                e.Cancel = true;
                string coords = url.Replace("openmap://", "").TrimEnd('/');
                var parts = coords.Split(',');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lng))
                {
                    MainThread.BeginInvokeOnMainThread(async () => {
                        var location = new Location(lat, lng);
                        var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                        try { await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options); }
                        catch { await DisplayAlert("Lỗi", LocalizationResourceManager.Instance["Không thể mở ứng dụng bản đồ."], "OK"); }
                    });
                }
            }
        }

        private async void BottomInfoCard_Tapped(object sender, EventArgs e)
        {
            int targetId = _selectedPoiId > 0 ? _selectedPoiId : (_nearestPoi?.Id ?? 0);

            if (targetId > 0)
            {
                var poi = _cachedPois.FirstOrDefault(p => p.Id == targetId);
                if (poi != null)
                {
                    var foodPlace = ConvertPoiToFoodPlace(poi);
                    await Navigation.PushAsync(new PlaceDetailPage(foodPlace));
                }
            }
            else
            {
                await DisplayAlert("Thông báo", LocalizationResourceManager.Instance["Chọn một điểm trên map"] ?? "Vui lòng chạm chọn một quán trên bản đồ.", "OK");
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

                if (location == null)
                {
                    location = new Location(10.761622, 106.661172);
                }

                if (location != null) Geolocation_LocationChanged(this, new GeolocationLocationChangedEventArgs(location));
            }
            catch { }
        }

        private void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            _currentUserLocation = e.Location;

            Task.Run(async () =>
            {
                // Lấy từ danh sách thật. Nếu danh sách rỗng (chưa có mạng), thì radar nghỉ ngơi, không quét nữa.
                var pois = _cachedPois;
                if (pois == null || !pois.Any()) return;

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
                    try
                    {
                        var latStr = _currentUserLocation.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        var lngStr = _currentUserLocation.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        await BanDoWebView.EvaluateJavaScriptAsync($"setUserLocation({latStr}, {lngStr});");

                        if (DebugLabel != null) DebugLabel.Text = $"GPS: {_currentUserLocation.Latitude:F5}, {_currentUserLocation.Longitude:F5}";

                        if (_selectedPoiId == 0)
                        {
                            CardStatusLabel.RemoveBinding(Label.TextProperty);
                            CardStatusLabel.SetBinding(Label.TextProperty, new Binding("CurrentLanguageCode", source: VinhKhanhTour.Services.LocalizationResourceManager.Instance, converter: VinhKhanhTour.Helpers.TranslateConverter.Instance, converterParameter: "BẠN ĐANG Ở GẦN"));

                            if (DistanceLabel != null)
                            {
                                DistanceLabel.RemoveBinding(Label.TextProperty);
                                DistanceLabel.Text = $"{Services.LocalizationResourceManager.Instance["Cách quán gần nhất"] ?? "Cách quán gần nhất"}: {nearestDistance:F0}m";
                            }

                            if (PoiNameLabel != null)
                            {
                                PoiNameLabel.RemoveBinding(Label.TextProperty);
                                string notFoundText = Services.LocalizationResourceManager.Instance["Chưa tìm thấy quán ốc"] ?? "Chưa tìm thấy quán ốc";
                                PoiNameLabel.Text = nearestTriggeredPoi != null ? nearestTriggeredPoi.Name : (closestPoi?.Name ?? notFoundText);
                            }
                        }

                        if (nearestTriggeredPoi != null)
                        {
                            if (_currentDwellingPoiId != nearestTriggeredPoi.Id)
                            {
                                _currentDwellingPoiId = nearestTriggeredPoi.Id;
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
                    }
                    catch { }
                });
            });
        }

        private FoodPlace ConvertPoiToFoodPlace(Poi p)
        {
            return new FoodPlace { Id = p.Id.ToString(), Name = p.Name, NarrationText = p.DisplayTtsScript, ImageUrl = p.ImageUrl };
        }

        private async void OnRouteClicked(object sender, EventArgs e)
        {
            Poi target = (_selectedPoiId > 0) ? _cachedPois.FirstOrDefault(p => p.Id == _selectedPoiId) : _nearestPoi;

            if (target != null)
            {
                try
                {
                    var location = new Location(target.Latitude, target.Longitude);
                    var options = new MapLaunchOptions { Name = target.Name, NavigationMode = NavigationMode.Driving };
                    await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options);
                }
                catch { await DisplayAlert("Lỗi", LocalizationResourceManager.Instance["Không thể mở ứng dụng bản đồ."] ?? "Lỗi", "OK"); }
            }
            else
            {
                await DisplayAlert("Chú ý", LocalizationResourceManager.Instance["Chọn một điểm trên map"] ?? "Vui lòng chọn 1 điểm", "OK");
            }
        }

        private async void OnPlayPauseClicked(object sender, EventArgs e)
        {
            Poi targetPoi = (_selectedPoiId > 0) ? _cachedPois.FirstOrDefault(p => p.Id == _selectedPoiId) : _nearestPoi;

            if (targetPoi == null)
            {
                await DisplayAlert("Thông báo", LocalizationResourceManager.Instance["Chọn một điểm trên map"] ?? "Vui lòng chọn 1 điểm", "OK");
                return;
            }

            if (_isPlaying)
            {
                _isPlaying = false;
                if (PlayPauseButton != null)
                {
                    PlayPauseButton.Text = "▶️";
                    PlayPauseButton.BackgroundColor = Color.FromArgb("#FF5C0F");
                }
                NarrationEngine.Instance.CancelCurrentNarration();
                await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({targetPoi.Id}, '{(_selectedPoiId == targetPoi.Id ? "selected" : "normal")}');");
            }
            else
            {
                _isPlaying = true;
                if (PlayPauseButton != null)
                {
                    PlayPauseButton.Text = "⏹";
                    PlayPauseButton.BackgroundColor = Color.FromArgb("#E53935");
                }
                await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({targetPoi.Id}, 'playing');");

                try
                {
                    var foodPlace = ConvertPoiToFoodPlace(targetPoi);
                    await NarrationEngine.Instance.PlayNarrationAsync(foodPlace, isManual: true);
                }
                finally
                {
                    if (_isPlaying)
                    {
                        _isPlaying = false;
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            if (PlayPauseButton != null)
                            {
                                PlayPauseButton.Text = "▶️";
                                PlayPauseButton.BackgroundColor = Color.FromArgb("#FF5C0F");
                            }
                            try { await BanDoWebView.EvaluateJavaScriptAsync($"updateMarkerState({targetPoi.Id}, '{(_selectedPoiId == targetPoi.Id ? "selected" : "normal")}');"); }
                            catch { }
                        });
                    }
                }
            }
        }

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

            var tab1 = CreateTabItem("Trang chủ", "🏠", false, async () => await Navigation.PushAsync(new HomePage(), false));
            Grid.SetColumn(tab1, 0);
            tabBarGrid.Children.Add(tab1);

            var tab2 = CreateTabItem("Bản đồ", "🗺️", true, null);
            Grid.SetColumn(tab2, 1);
            tabBarGrid.Children.Add(tab2);

            var tab3 = CreateTabItem("Cài đặt", "⚙️", false, async () => await Navigation.PushAsync(new SettingsPage(), false));
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

        private View CreateTabItem(string text, string iconText, bool isSelected, Action action)
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

            if (action != null)
            {
                layout.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(action) });
            }

            return layout;
        }
    }
}