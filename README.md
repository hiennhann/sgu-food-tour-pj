
## Đồ án: Hệ Thống Du Lịch Ẩm Thực Vĩnh Khánh
**Thông tin chung:**
* **Nền tảng ứng dụng:** Mobile App dành cho Khách du lịch & Web Portal dành cho Quản lý/Chủ quán.
* **Kiến trúc Backend:** ASP.NET Core Web API.
* **Hệ quản trị CSDL:** PostgreSQL (tích hợp PostGIS xử lý tọa độ) & SQLite (Local DB).


## 1. TỔNG QUAN ĐỒ ÁN (Project Overview)
Hệ thống Du lịch Ẩm thực Vĩnh Khánh là một nền tảng chuyển đổi số toàn diện dành riêng cho khu phố ẩm thực Vĩnh Khánh (Quận 4). 
Dự án cung cấp một **bản đồ tương tác thông minh** trên thiết bị di động, hoạt động như một "hướng dẫn viên ảo", tự động phát thanh giới thiệu các quán ăn khi du khách đi dạo qua các địa điểm (dựa trên GPS). Đồng thời, hệ thống cung cấp một **Web Portal** giúp các chủ quán số hóa thực đơn, tiếp cận khách quốc tế qua AI dịch thuật, và giúp Ban quản lý kiểm duyệt, thống kê lượng khách tham quan.
## 2. ĐẶT VẤN ĐỀ (Problem Statement)
Khu phố Vĩnh Khánh rất sầm uất nhưng đang gặp phải các vấn đề cốt lõi:
1. **Đối với Du khách (Đặc biệt là khách quốc tế):**
   * **Rào cản ngôn ngữ:** Không hiểu biển hiệu, thực đơn tiếng Việt.
   * **Thiếu thông tin cục bộ:** Khó phân biệt được đặc sản của từng quán, dễ đi lạc hoặc bỏ lỡ các quán ngon.
   * **Phụ thuộc Internet:** Khách du lịch nước ngoài thường có kết nối mạng kém hoặc không ổn định khi đi dạo ngoài đường.
2. **Đối với Chủ quán:**
   * Không có công cụ chuyên nghiệp để quảng bá đa ngôn ngữ.
   * Khó khăn trong việc cập nhật menu, giá cả linh hoạt.
   * Khó tiếp nhận và quản lý đánh giá từ thực khách.
3. **Đối với Ban quản lý (Admin):**
   * Thiếu dữ liệu thống kê số hóa (không biết khách tập trung đông ở khu vực nào, quán nào được quan tâm nhất).
## 3. KIẾN TRÚC HỆ THỐNG (System Architecture)
* **Frontend (Mobile App):** Sử dụng C# với **.NET MAUI**, tích hợp SQLite để lưu trữ Local Database (đáp ứng cơ chế Offline-First).
* **Frontend (Web Portal):** Dashboard quản trị hiện đại dành cho Chủ quán và Admin.
* **Backend Core:** **ASP.NET Core Web API** xử lý logic nghiệp vụ, xác thực JWT, phân quyền RBAC.
* **Database:** **PostgreSQL** làm CSDL chính. Tích hợp extension **PostGIS** để lưu trữ và tính toán khoảng cách tọa độ, vùng địa lý (Geofence) của các quán ăn (POIs).
* **AI & External Services:** Tích hợp LLM (Gemini 2.0 Flash) để tối ưu mô tả quán/món ăn, Edge-TTS để tạo file âm thanh từ văn bản.
## 4. CÁC LUỒNG HOẠT ĐỘNG CHÍNH (Core Operation Flows)

### 4.1. Luồng Trải nghiệm của Khách Du Lịch (Mobile Flow)
1. **Khởi động & Cập nhật Offline:** Người dùng mở App $\rightarrow$ Hệ thống tải cấu hình và đồng bộ gói dữ liệu (Map Pack, Text, Audio cơ bản) về SQLite $\rightarrow$ Người dùng có thể tắt mạng và tiếp tục sử dụng.
2. **Khám phá (Discovery):** Xem bản đồ $\rightarrow$ Tìm kiếm quán ăn/món ăn $\rightarrow$ Lọc theo danh mục (Ốc, Lẩu, Sushi...) $\rightarrow$ Xem chi tiết quán và Menu đa ngôn ngữ.
3. **Trải nghiệm GPS & Geofence:** Du khách bỏ điện thoại vào túi $\rightarrow$ App đọc tọa độ GPS liên tục $\rightarrow$ Giao cắt với vùng Geofence (bán kính 20-30m) của một quán $\rightarrow$ App tự động phát Audio thuyết minh giới thiệu về quán đó.
4. **Tương tác:** Đánh giá (Rating/Review) và Lưu vào danh sách Yêu thích (Favorite).

### 4.2. Chiến lược Phân tầng (Tiers & Fallback Strategy)
Để giải quyết bài toán cốt lõi là **"Mạng yếu"** và **"Đa ngôn ngữ"**:
* **Chiến lược âm thanh 4 Tầng:** Phát MP3 Cache Local (0ms) $\rightarrow$ Dịch & Tạo Audio On-demand (2-5s) $\rightarrow$ Cloud Stream (3-8s) $\rightarrow$ Fallback dùng Local TTS của Hệ điều hành.
* **Phân tầng ngôn ngữ (3-Tier):** Ngôn ngữ đích của khách $\rightarrow$ Fallback Tiếng Anh $\rightarrow$ Fallback Tiếng Việt (chỉ hiện Text, tắt Audio).
## 5. DANH SÁCH TÍNH NĂNG & API ENDPOINTS CHI TIẾT

### 5.1. Nhóm Auth (Dùng chung cho Admin & Chủ quán)
* `POST /api/auth/login`: Đăng nhập (SĐT + Mật khẩu). Trả về JWT Token.
* `POST /api/auth/register-shop`: Đăng ký tài khoản chủ quán & gửi hồ sơ quán (Hồ sơ chuyển vào trạng thái chờ duyệt).
* `GET /api/auth/me`: Lấy thông tin tài khoản hiện tại để hiển thị trên Dashboard.

### 5.2. Nhóm Chủ Cửa Hàng (Shop Owner)
**Chức năng: Quản lý thông tin quán**
* `GET /api/owner/restaurant`: Lấy thông tin quán hiện tại.
* `PUT /api/owner/restaurant`: Cập nhật thông tin quán (Tên, địa chỉ, tọa độ GPS, mô tả).

**Chức năng: Quản lý món ăn**
* `GET /api/owner/foods`: Lấy danh sách món ăn của quán.
* `POST /api/owner/foods`: Thêm món ăn mới.
* `PUT /api/owner/foods/{id}`: Sửa thông tin món ăn (Tên, giá, mô tả).
* `DELETE /api/owner/foods/{id}`: Xóa món ăn (Áp dụng cơ chế xóa mềm - Soft delete).

**Chức năng: Xem đánh giá**
* `GET /api/owner/ratings`: Xem danh sách đánh giá, bình luận từ khách du lịch.
* `GET /api/owner/ratings/stats`: Xem điểm trung bình sao của quán.

**Chức năng: Upload Media**
* `POST /api/owner/upload`: API dùng chung để upload ảnh không gian quán, ảnh món ăn lên server (Trả về URL ảnh). 
### 5.3. Nhóm Admin (Quản trị viên) -
**Chức năng: Quản lý cửa hàng**
* `GET /api/admin/restaurants`: Lấy danh sách tất cả các quán trên hệ thống.
* `GET /api/admin/restaurants/pending`: Danh sách các quán đang chờ duyệt.
* `PUT /api/admin/restaurants/{id}/approve`: Duyệt quán, kích hoạt hiển thị trên App.
* `PUT /api/admin/restaurants/{id}/reject`: Từ chối quán (Yêu cầu gửi kèm lý do).

**Chức năng: Quản lý người dùng**
* `GET /api/admin/users`: Danh sách toàn bộ người dùng (Chủ quán, Khách du lịch, v.v.).
* `PUT /api/admin/users/{id}/status`: Khóa hoặc Mở khóa tài khoản người dùng vi phạm.

**Chức năng: Quản lý nội dung**
* `GET /api/admin/narrations`: Quản lý các bản text thuyết minh (Admin có quyền chỉnh sửa/kiểm duyệt văn phong trước khi tạo Audio).
* `DELETE /api/admin/ratings/{id}`: Xóa các đánh giá vi phạm tiêu chuẩn cộng đồng (spam, ngôn từ tục tĩu).

**Chức năng: Thống kê hệ thống**
* `GET /api/admin/stats/overview`: Thống kê tổng quan số lượng quán, tổng người dùng, tổng lượt nghe thuyết minh.
* `GET /api/admin/stats/heatmap`: Lấy dữ liệu tọa độ truy cập của du khách để vẽ bản đồ nhiệt (Heatmap) trên giao diện Dashboard.
