# Hệ Thống Du Lịch Ẩm Thực Phố Vĩnh Khánh Quận 4

<p align="center">
  <img src="https://img.shields.io/badge/PROJECT-VINH%20KHANH%20TOUR-FF7327?style=for-the-badge" alt="Project Badge">
  <img src="https://img.shields.io/badge/.NET-9.0-512bd4?style=for-the-badge&logo=dotnet" alt=".NET 9.0">
  <img src="https://img.shields.io/badge/.NET%20MAUI-MOBILE%20APP-008ACA?style=for-the-badge&logo=dotnet" alt=".NET MAUI">
  <img src="https://img.shields.io/badge/SQLITE-DATABASE-003B57?style=for-the-badge&logo=sqlite" alt="SQLite">
</p>

## 1. Mô tả ứng dụng
Ứng dụng di động đa nền tảng đóng vai trò như một "hướng dẫn viên cá nhân" tại khu phố ẩm thực Vĩnh Khánh (Quận 4). Ứng dụng cung cấp bản đồ tương tác, tự động phát âm thanh (Text-to-Speech) giới thiệu các quán ăn theo vị trí thực (GPS) và được tối ưu hóa để hoạt động tốt với dữ liệu lưu trữ cục bộ.

## 2. Các công nghệ được sử dụng (Tech Stack)
- **Nền tảng ứng dụng**: .NET MAUI (hỗ trợ Android, iOS, Windows, MacCatalyst).
- **Ngôn ngữ lập trình chính**: C#, XAML cho giao diện gốc.
- **Công nghệ Bản đồ (Mapping)**: Sử dụng WebView tích hợp HTML/JS/CSS với thư viện **Leaflet.js** (nguồn bản đồ CartoCDN). Xử lý tương tác hai chiều mượt mà giữa JavaScript và C# MAUI thông qua URL Schemes tùy chỉnh (`tappin://`, `mapclick://`).
- **Cơ sở dữ liệu cục bộ**: **SQLite** (`sqlite-net-pcl`) được sử dụng để lưu trữ dữ liệu địa điểm (POIs) và các thiết lập, cung cấp khả năng truy xuất ngoại tuyến nhanh chóng.
- **Cảm biến và GPS**: **MAUI Essentials Geolocation** đảm nhiệm việc theo dõi tọa độ người dùng liên tục theo thời gian thực.
- **Thuyết minh Âm thanh**: **MAUI Essentials Text-to-Speech (TTS)** sử dụng công cụ tổng hợp giọng nói mặc định của thiết bị để đọc kịch bản.

## 3. Các chức năng chính của Project
- **Bản đồ Ẩm thực Tương tác**: Cắm ghim (pins) danh sách các quán ăn nổi bật. Cho phép người dùng chạm để xem sơ lược về khoảng cách và thông tin quán.
- **Hàng Rào Địa Lý & Thuyết Minh Tự Động (Geofencing & Auto TTS)**: Ứng dụng vẽ sẵn các "vùng radar" (bán kính) xung quanh mỗi quán ăn. Khi thiết bị di chuyển vào vùng này, hệ thống sẽ tự động kích hoạt đọc nội dung thuyết minh về quán ăn mà không cần bất cứ thao tác bấm nào từ người dùng.
- **Theo dõi Vị trí (GPS Tracking)**: Liên tục cập nhật và hiển thị biểu tượng vị trí của người sử dụng trên bản đồ, tính toán khoảng cách theo đường chim bay từ người dùng tới tất cả các quán ăn trong khu vực.
- **Hỗ trợ Offline-first**: Dữ liệu danh sách quán và kịch bản khởi tạo được lưu (seed) ngay vào SQLite từ lúc bắt đầu. Giúp ứng dụng ít phải phụ thuộc vào internet (chỉ cần net để load tiles bản đồ).
- **Chỉ đường Nhanh (Navigation)**: Liên kết trực tiếp tọa độ quán ăn trên app sang các ứng dụng bản đồ như Google Maps hay Apple Maps để điều hướng chi tiết.
- **Đa ngôn ngữ (Localization)**: Hệ thống `LocalizationResourceManager` linh hoạt giúp thay đổi text trên UI dựa theo cài đặt ngôn ngữ mong muốn.

## 4. Luồng hoạt động của hệ thống
1. **Khởi chạy ứng dụng**: Ứng dụng kiểm tra và khởi tạo SQLite Database. Nếu là lần đầu chạy (hoặc khi có cập nhật version), nó sẽ tự động seed dữ liệu mẫu (các quán ăn Vĩnh Khánh) vào thiết bị.
2. **Khám phá Bản đồ**: Màn hình MapPage nạp các tọa độ từ SQLite và truyền dữ liệu xuống giao diện Leaflet.js qua chuỗi HTML để vẽ các Marker và Vùng Radar.
3. **Tracking & Geofencing**: Ngay khi được cấp quyền định vị, một luồng (Timer) chạy ngầm sẽ kiểm tra GPS mỗi 5 giây. Vị trí người dùng được đẩy vào bản đồ HTML để cập nhật điểm xanh.
4. **Trải nghiệm Thuyết minh**:
   - Khi hàm `CalculateDistanceInMeters` nhận diện vị trí người dùng cách quán ăn <= Bán kính cấu hình.
   - Ứng dụng gọi JavaScript đổi Marker sang trạng thái "Đang phát (Vàng)".
   - Gọi `NarrationEngine.PlayNarrationAsync` để bắt đầu đọc kịch bản Text-to-Speech.
   - Có cơ chế chống spam (Cooldown 2 phút) khi người dùng cứ đứng lấp lửng ở ranh giới của vùng Radar.
