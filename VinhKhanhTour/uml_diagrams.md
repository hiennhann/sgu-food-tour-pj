# Sơ đồ UML - Vinh Khanh Food Tour App

Dưới đây là các sơ đồ Use Case, Activity, và Sequence cho các chức năng chính của ứng dụng được trích xuất từ source code, bao gồm cả các chức năng điều hướng chi tiết như Xem Thực Đơn và Chỉ Đường bằng hệ thống Maps ngoài.

## 1. Sơ đồ Use Case Tổng quan

> [!NOTE]
> Sơ đồ này thể hiện các tương tác chính của Người dùng (Tourist) với hệ thống. Đã bổ sung đầy đủ luồng Mở Menu và Chỉ đường.

```mermaid
flowchart LR
    Người_dùng(["Người dùng (Tourist)"])

    subgraph App ["Vinh Khanh Food Tour App"]
        direction TB
        UC1(["Đăng nhập / Đăng ký"])
        UC2(["Xem danh sách địa điểm"])
        UC3(["Lọc địa điểm theo danh mục"])
        UC4(["Xem chi tiết địa điểm"])
        UC5(["Xem bản đồ"])
        UC6(["Theo dõi vị trí (GPS Radar)"])
        UC7(["Nghe Audio giới thiệu"])
        UC8(["Đổi ngôn ngữ / Cài đặt"])
        UC9(["Xem thực đơn"])
        UC10(["Chỉ đường (Google Maps)"])
    end

    Người_dùng --> UC1
    Người_dùng --> UC2
    Người_dùng --> UC4
    Người_dùng --> UC5
    Người_dùng --> UC8

    UC2 -. "<<extend>>" .-> UC3
    UC4 -. "<<include>>" .-> UC7
    UC4 -. "<<extend>>" .-> UC9
    UC4 -. "<<extend>>" .-> UC10
    UC5 -. "<<include>>" .-> UC6
    UC5 -. "<<extend>>" .-> UC7
    UC5 -. "<<extend>>" .-> UC10
```

---

## 2. Các Sơ đồ cho chức năng Xem Chi Tiết Địa Điểm (Mới bổ sung)

### Activity Diagram (Xem Chi Tiết, Gọi Món, Chỉ Đường)

```mermaid
flowchart TD
  A([Bắt đầu mở Màn hình Chi tiết Quán]) --> B[Hiển thị thông tin tổng quan, Rating]
  B --> C{Lựa chọn của người dùng?}
  C -- Bấm nút 'Thực Đơn' --> D[Mở Màn hình MenuPage]
  D --> E[Truy vấn dữ liệu món ăn từ MenuDataStore]
  E --> F[Hiển thị danh sách món ăn và giá]
  F --> K([Đóng màn hình / Kết thúc])
  
  C -- Bấm nút 'Nghe Audio' --> G[Mở Màn hình AudioPlayerPage]
  G --> H{Thao tác trong AudioPlayer?}
  H -- Bấm 'Nghe Audio' / 'Dừng' --> I[Gọi NarrationEngine Singleton xử lý đọc Text]
  H -- Bấm nút 'Chỉ đường' --> J[Gọi API Map của Hệ điều hành mở GG Maps]
  H -- Bấm nút 'Thực Đơn' --> D
  J --> K
  I --> K
```

---

## 3. Các Sơ đồ cho chức năng Đăng nhập / Đăng ký

### Activity Diagram (Đăng nhập)

```mermaid
flowchart TD
  A([Bắt đầu]) --> B[Nhập Email và Mật khẩu]
  B --> C[Bấm nút ĐĂNG NHẬP]
  C --> D[Truy vấn DatabaseService lấy User]
  D --> E{User tồn tại và đúng mật khẩu?}
  E -- Đúng --> F[Lưu trạng thái đăng nhập vào AppState]
  F --> G[Lưu UserName và UserEmail]
  G --> H[Đóng màn hình Login / PopAsync]
  H --> I([Kết thúc])
  E -- Sai --> J[Hiển thị lỗi 'Email hoặc mật khẩu không đúng!']
  J --> I
```

### Sequence Diagram (Đăng nhập)

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant P as LoginPage
    participant DB as DatabaseService
    participant A as AppState

    U->>P: Nhập Email, Password
    U->>P: Bấm "ĐĂNG NHẬP"
    P->>DB: GetUserAsync(email, password)
    activate DB
    DB-->>P: User object / null
    deactivate DB

    alt User hợp lệ
        P->>A: Cập nhật IsLoggedIn = true
        P->>A: Cập nhật UserName, UserEmail
        P->>P: Navigation.PopAsync()
        P-->>U: Trở về màn hình trước đó
    else User không hợp lệ
        P->>P: DisplayAlert("Thất bại")
        P-->>U: Hiển thị thông báo lỗi
    end
```

---

## 4. Các Sơ đồ cho chức năng Xem danh sách địa điểm (Home Page)

### Activity Diagram (Xem & Lọc)

```mermaid
flowchart TD
  A([Bắt đầu]) --> B[Mở màn hình HomePage]
  B --> C[Tạo danh sách Filter: Tất cả, Ốc, Đồ nướng]
  C --> D[Load FeaturedPlaces từ ViewModel]
  D --> E{Người dùng có chọn Filter?}
  E -- Có --> F[Gửi sự kiện OnCategorySelected]
  F --> G[ViewModel lọc lại danh sách món ăn]
  G --> H[Cập nhật UI CollectionView]
  E -- Không --> I[Hiển thị danh sách ban đầu]
  H --> I
  I --> J{Hành động tiếp theo?}
  J -- Bấm thẻ quán ăn --> K[Mở PlaceDetailPage]
  J -- Bấm nút Nghe Audio nhỏ gọn --> L[Hủy Audio hiện tại]
  L --> M[Phát Audio giới thiệu nhanh ngay trên List]
  K --> N([Kết thúc])
  M --> N
```

### Sequence Diagram (Load & Lọc Địa Điểm)

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant V as HomePage
    participant VM as HomeViewModel
    participant S as LocalizationResourceManager

    U->>V: Mở App (HomePage)
    V->>VM: Khởi tạo HomeViewModel()
    activate VM
    VM->>VM: Load FeaturedPlaces (Mock/DB)
    VM-->>V: Binding FeaturedPlaces trả về
    deactivate VM
    V->>S: Get Dịch thuật các Label
    V-->>U: Hiển thị danh sách quán ăn

    U->>V: Chọn nhóm lọc (VD: "Ốc & Hải sản")
    V->>V: Update UI nút lọc (Màu cam)
    V->>VM: FilterByCategory(categoryName)
    activate VM
    VM->>VM: Filter FeaturedPlaces list
    VM-->>V: CollectionView tự động update qua Binding
    deactivate VM
    V-->>U: Hiển thị danh sách đã lọc
```

---

## 5. Các Sơ đồ cho chức năng Bản đồ & Theo dõi Vị trí (Map Page)

### Activity Diagram (Map & GPS Radar)

```mermaid
flowchart TD
  A([Bắt đầu vào MapPage]) --> B[Tải dữ liệu POI từ DB/Mock]
  B --> C[Tạo HTML JS Bản đồ Leaflet Inject lên WebView]
  C --> D[Yêu cầu quyền GPS LocationWhenInUse]
  D --> E{Được cấp quyền GPS?}
  E -- Có --> F[Bật Timer chu kỳ quét mội 5s]
  E -- Không --> G[Dừng theo dõi vị trí]
  G --> L([Kết thúc luồng Background])
  F --> H[Lấy GPS hiện tại GetLocationAsync]
  H --> I[Tính khoảng cách đến các POI có trên Map]
  I --> J[Cập nhật Vị trí User lên Bản đồ qua WebView JS]
  J --> K{Có POI nào nằm trong bán kính?}
  K -- Có --> M[Cập nhật Label báo hiệu: Đang ở gần...]
  K -- Không --> N[Cập nhật Label báo hiệu: Đang quét Radar...]
  M --> O[Đợi 5s]
  N --> O
  O --> H
```

### Sequence Diagram (Quét Radar & Tương tác Bản đồ)

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant M as MapPage
    participant GEO as Geolocation
    participant WV as BanDoWebView (JS)
    participant EXT as Google Maps (App Khác)
    
    M->>M: OnNavigatedTo
    M->>GEO: Check/Request Quyền Location
    GEO-->>M: Granted
    M->>M: Start Timer 5 giây
    
    loop Cứ mỗi 5 giây
        M->>GEO: GetLocationAsync()
        GEO-->>M: Cung cấp Tọa độ (Lat, Lng)
        M->>M: Tính toán khoảng cách (Radar)
        M->>WV: EvaluateJavaScriptAsync(setUserLocation(lat, lng))
        M->>M: Update UI (Labels báo vị trí, khoảng cách)
    end

    U->>WV: Tap vào 1 Marker quán ốc trên Bản đồ (JS)
    WV->>M: WebNavigating (tappin://{id})
    M->>M: Lưu lại _selectedPoiId
    M->>M: Cập nhật CardStatusLabel & Hiện nút Audio
    M->>WV: EvaluateJavaScriptAsync(updateMarkerState)
    
    U->>M: Tap vào Nút "Chỉ Đường" dưới Card
    M->>EXT: Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync()
    EXT-->>U: Hiển thị tuyến đường (Google Maps/Apple Maps)
```

---

## 6. Các Sơ đồ cho chức năng Phát Audio Giới Thiệu

### Activity Diagram (Phát Audio bởi NarrationEngine)

```mermaid
flowchart TD
  A([Bắt đầu yêu cầu Nghe Audio]) --> B{Trạng thái Engine hiện tại?}
  B -- Đang phát --> C[Hủy Cancel Narration hiện tại]
  C --> D[Đồng bộ nút UI đang Stop ⏹ về Play ▶️]
  D --> F([Kết thúc])
  B -- Chưa phát --> G[Đồng bộ nút UI từ Play ▶️ thành Stop ⏹]
  G --> H[Đồng bộ UI Map sang nháy vàng (nếu dùng chung Map)]
  H --> I[Gọi NarrationEngine Singleton xử lý]
  I --> J[Trích xuất Text diễn đọc từ đối tượng Model]
  J --> K[Gọi TextToSpeech.Default.SpeakAsync]
  K --> L[Chờ thiết bị TTS đọc xong hoặc có Cancel Token Error]
  L --> M[Restore trạng thái UI Button về ▶️]
  M --> N[Restore UI Map Marker về Bình thường]
  N --> F
```

### Sequence Diagram (NarrationEngine phát Audio)

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant P as Bất kì Trang nào (Map, List, AudioPlayer)
    participant NE as NarrationEngine(Singleton)
    participant TTS as OS TextToSpeech API
    
    U->>P: Bấm ▶️ Nghe Audio
    P->>P: Cập nhật UI (nút đổi thành ⏹)
    P->>NE: CancelCurrentNarration()
    P->>NE: PlayNarrationAsync(FoodPlace)
    activate NE
    NE->>NE: Khởi tạo CancellationToken mới
    NE->>NE: Parsing Text cần đọc
    NE->>TTS: SpeakAsync(text, cancelToken)
    activate TTS
    
    alt User bấm Dừng ⏹ giữa chừng
        U->>P: Bấm ⏹ Dừng lại
        P->>NE: CancelCurrentNarration()
        NE->>TTS: Bắn Cancel Signal
        TTS-->>NE: Exception/Dừng Task TextToSpeech
    else Không ngắt ngang, chờ đọc xong
        TTS-->>NE: Phát hoàn tất thành công (Completed Task)
    end
    deactivate TTS
    
    NE-->>P: PlayNarrationAsync done
    deactivate NE
    P->>P: Trigger lệnh Restore UI (nút trở lại ▶️ màu cam)
```
