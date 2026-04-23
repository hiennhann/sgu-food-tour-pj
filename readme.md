# 🗺️ SGU Food Tour - Code Architecture Mapping

Tài liệu này đóng vai trò như một **bản đồ chỉ đường (Mapping)** để giúp giảng viên hoặc các thành viên trong nhóm dễ dàng đối chiếu giữa **Sơ đồ UML (VinhKhanhTour_UML_Updated.html)** và **Vị trí các File Code Vật lý** trong thư mục dự án.

---

## F1. Hệ Thống Đồng Bộ Cấu Trúc Json (Offline Database Sync)
Chức năng này chịu trách nhiệm kéo dữ liệu ban đầu từ API về lưu vào SQLite.

* **Mobile App:**
  * File: `VinhKhanhTour/Services/ApiService.cs`
    * Hàm: `SyncDataAsync()` - Gửi Request HTTP GET để lấy JSON POI/Tour.
  * File: `VinhKhanhTour/Services/LocalDbService.cs`
    * Hàm: `InitAsync()` - Khởi tạo kết nối SQLite.
    * Hàm: `DeleteAllAsync<T>()` - Xóa sạch DB cũ.
    * Hàm: `InsertAllAsync<T>()` - Ghi đè CSDL Offline mới.
* **Web CMS Backend:**
  * File: `VinhKhanhTour.CMS/Controllers/ApiApiController.cs`
    * Hàm: `GetPois()` - Trả về dữ liệu JSON Cảnh Điểm cho Mobile.

---

## F2. Hệ Thống Paywall Trải Nghiệm & Đăng Nhập Device
Quản lý quyền sử dụng ứng dụng thông qua mã thiết bị (DeviceID).

* **Mobile App:**
  * File: `VinhKhanhTour/Services/SubscriptionService.cs`
    * Hàm: `GetOrCreateDeviceIdAsync()` - Đọc UUID duy nhất của điện thoại.
    * Hàm: `CheckStatusAsync()` - Gọi API kiểm tra hạn dùng 24h.
  * File: `VinhKhanhTour/Views/PaywallPage.xaml.cs` (hoặc `.cs`)
    * Xử lý UI khóa màn hình và hiện mã QR thanh toán.
* **Web CMS Backend:**
  * File: `VinhKhanhTour.CMS/Controllers/SubscriptionController.cs`
    * Hàm: `CheckDeviceStatus()` - Logic tính toán mốc 24h từ `FirstLaunchDate`.
    * Hàm: `ApproveDevice()` - Hàm duyệt chuyển khoản từ Admin để set `IsPaid = True`.

---

## F3. Khám Phá Trang Chủ, Tìm Kiếm & Đổi Ngôn Ngữ
Logic tải hiển thị danh sách quán ăn, khử dấu tiếng Việt và thay đổi Culture.

* **Mobile App:**
  * File: `VinhKhanhTour/ViewModels/HomeViewModel.cs`
    * Hàm: `LoadDataAsync()` - Nạp dữ liệu offline từ Repository lên UI.
    * Hàm: `SearchByName()` - Tìm kiếm quán ăn.
    * Hàm: `RemoveDiacritics()` - Thuật toán khử dấu Tiếng Việt phục vụ ô SearchBar.
  * File: `VinhKhanhTour/Services/PoiRepository.cs`
    * Hàm: `GetAllPoisAsync()` - Query từ Local SQLite DB.
  * File: `VinhKhanhTour/Helpers/LocalizationResourceManager.cs`
    * Quản lý biến tĩnh `CurrentLanguageCode` và hàm `SetCulture()` để thay đổi UI ngay lập tức không cần tải lại app.

---

## F4. Bản Đồ Geofencing & Tự Động Kích Hoạt Âm Thanh
Tính năng radar chạy nền kiểm tra tọa độ.

* **Mobile App:**
  * File: `VinhKhanhTour/Views/MapPage.cs`
    * Hàm: `StartLocationTracking()` - Khởi tạo vòng lặp Timer.
    * Hàm: `CalculateDistance()` - Thuật toán Toán học tính khoảng cách giữa 2 tọa độ GPS (Euclide / Haversine).
  * File: `VinhKhanhTour/Services/NarrationEngine.cs`
    * Hàm: `PlayNarrationAsync()` - Gửi Script vào OS TTS Engine để đọc lên.
    * Hàm: `CancelCurrentNarration()` - Cắt âm thanh nếu khách đi ra khỏi bán kính quán.

---

## F5. Bản Đồ Nhiệt Hiển Thị Đám Đông (SignalR Heatmap)
Theo dõi vị trí tập trung đông người thời gian thực.

* **Web CMS Backend:**
  * File: `VinhKhanhTour.CMS/Hubs/MonitoringHub.cs`
    * Dictionary: `ConcurrentDictionary<string, DeviceInfo> OnlineDevices` - Lưu trữ tọa độ khách trên RAM Server.
    * Hàm: `UpdateUserLocation(int poiId)` - Mobile bắn vị trí lên đây.
    * Hàm: `BroadcastHeatmap()` - Gửi mảng đếm (Count) số người tại từng POI xuống toàn bộ mạng lưới thiết bị.
* **Mobile App:**
  * File: `VinhKhanhTour/Views/MapPage.cs`
    * Hàm: `InitSignalR()` - Kết nối Hub.
    * Sự kiện: `hubConnection.On("UpdateHeatmap")` - Lắng nghe thay đổi mật độ để đổi màu Cảnh báo Giao thông (Xanh/Vàng/Đỏ).

---

## F6. Xem Chi Tiết Quán Và Audio Thủ Công
* **Mobile App:**
  * File: `VinhKhanhTour/Views/PlaceDetailPage.cs`
    * Mở Google Maps bằng thư viện `AppInfo.ShowMapAsync(Location)`.
  * File: `VinhKhanhTour/Views/AudioPlayerPage.cs`
    * Giao diện trình phát nhạc, có thể play file .MP3 vật lý từ MediaElement hoặc fallback về hàm TTS của `NarrationEngine`.

---

## F7. Quản Trị Hệ Thống Backend (CMS Admin)
Môi trường Web dành cho quản trị viên thêm dữ liệu.

* **Web CMS Backend:**
  * Cấu hình DB: `VinhKhanhTour.CMS/Data/AppDbContext.cs` (Entity Framework PostgreSQL).
  * Quản lý Điểm & Tour: `VinhKhanhTour.CMS/Controllers/PoiController.cs` và `TourController.cs` (CRUD cơ bản).
  * Upload File Âm thanh: `VinhKhanhTour.CMS/Controllers/AudioTrackController.cs` 
    * Hàm `UploadAudio(IFormFile file)` - Kiểm tra dung lượng < 5MB, ghi FileStream vào thư mục `wwwroot/audio` và lưu Path CSDL.

---
*Được biên soạn tự động từ tài liệu kiến trúc UML VinhKhanhTour_UML_Updated.html*
