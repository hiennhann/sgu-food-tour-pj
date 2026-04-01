Hệ Thống Du Lịch Ẩm Thực Phố Vĩnh Khánh Quận 4
1.	Mô tả ứng dụng
Thuyết minh đa ngôn ngữ các cửa hàng ẩm thực phở phố Vĩnh Khánh, tương tác với bản đồ dễ dàng, hoạt động Offline-first với chi phí API cốt lõi là 0 VNĐ.
2.	Đặt vấn đề
Phố ẩm thực Vĩnh Khánh rất sầm uất nhưng đang gặp phải các vấn đề cốt lõi như:
a.	Đối với Du khách ( đặt biệt là khách quốc tế):
Rào cản ngôn ngữ: Không hiểu biển hiệu, thực đơn bằng tiếng Việt.
Thiếu thông tin cục bộ: Khó phân biệt được đặc sản của từng quán, dễ đi lạc hoặc bỏ lỡ các quán hợp gu ăn uống.
Phụ thuộc vào Internet: Khách du lịch nước ngoài thường có kết nối mạng kém hoặc không ổn định khi đi dạo ngoài đường.
b.	Đối với Chủ Quán:
Không có công cụ chuyên nghiệp để quảng bá đa ngôn ngữ
Khó khăn trong việc cập nhật menu, giá cả 
Khó tiếp cận khách hàng online, ghi nhận ý kiến khách hàng.
3.	Tổng quan hệ sinh thái
Nội dung: hình ảnh, load-all API
Âm thanh: Edge-TTS, trình dịch sâu, AudioTaskManager, SSE.
Bản địa hóa: Khởi động poi-localizations
Bản đồ: PMTiles, tệp kê khai, phông chữ, sprite, bảo vệ Path Traversal.
4.	Kiến trúc hệ thống
Frontend (Mobile App): Sử dụng ngôn ngữ lập trình C-Sharp tích hợp SQL-lite ( dùng để đáp ứng cơ chế Offline-first)
Backend Core: ASP.NET xử lý logic nghiệp vụ.
5.	Các luồng hoạt động chính
Luồng trải nghiệm của khách du lịch
a.	Khởi động và cập nhật Offline:
Người dùng mở app -> hệ thống tải cấu hình và đồng bộ gói dữ liệu ( Map, Text, Audio cơ bản) về SQLite -> người dùng có thể tắt mạng và tiếp tục sử dụng.
b.	Khám phá:
Xem bản đồ -> tìm kiếm quán ăn/món ăn -> lọc theo danh mục -> xem chi tiết quán và menu đa ngôn ngữ
c.	Trải nghiệm GPS & Geofence: Du khách bỏ điện thoại vô túi -> App đọc tọa độ liên tục -> giao cắt với vùng Genfence ( bán kính khoảng 30m) của một quán -> App tự động phát Audio để thuyết minh giới thiệu về quán đó.
d.	Tương tác:
Đánh giá ( Review/Rating) và có thể lưu vào danh sách quán ăn yêu thích.
