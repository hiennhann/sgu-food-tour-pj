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

/// mới
1. Tóm tắt dự án (Executive Summary)
Xây dựng một ứng dụng di động thông minh nhằm số hóa trải nghiệm du lịch ẩm thực tại phố Vĩnh Khánh. Ứng dụng hoạt động như một "hướng dẫn viên cá nhân", cung cấp bản đồ tương tác, thuyết minh đa ngôn ngữ tự động (Text-to-Speech) dựa trên vị trí (GPS), với tiêu chí hoạt động Offline-first (không cần Internet liên tục) và chi phí API cốt lõi là 0 VNĐ.

Mô hình kinh doanh (Monetization): Ứng dụng áp dụng mô hình Freemium / In-App Purchase (IAP). Người dùng có thể tải app miễn phí để xem giới thiệu, nhưng cần thanh toán phí (một lần) qua cửa hàng ứng dụng để mở khóa trọn bộ tính năng bản đồ ngoại tuyến và kho dữ liệu thuyết minh âm thanh. Ứng dụng không có hệ thống quản trị dành cho bên thứ ba (Chủ quán/Admin).

2. Đặt vấn đề & Mục tiêu (Problem & Goals)
Nỗi đau của Du khách (Đặc biệt là khách quốc tế):

Rào cản ngôn ngữ: Không hiểu biển hiệu, thực đơn, khó phân biệt các món ốc/hải sản địa phương.

Thiếu thông tin cục bộ: Khó phân biệt đặc sản của từng quán, dễ đi lạc hoặc bỏ lỡ các quán nổi bật, thiếu câu chuyện văn hóa phía sau món ăn.

Phụ thuộc Internet: Kết nối 4G/Wifi thường chập chờn khi di chuyển ngoài đường phố, gây gián đoạn trải nghiệm tra cứu.

Mục tiêu Giải pháp:
Mang lại trải nghiệm liền mạch, tự động hóa việc cung cấp thông tin tại đúng địa điểm, đúng thời điểm mà không cần thao tác thủ công từ người dùng, đồng thời đem lại doanh thu trực tiếp cho nhà phát triển.

3. Chân dung người dùng (Target Audience)
Khách du lịch tự túc (Backpackers): Yêu thích tự do khám phá, sẵn sàng chi trả khoản phí nhỏ cho một tour guide tự động chất lượng cao, cần ứng dụng nhẹ, chạy offline tốt.

Khách quốc tế (Expats/Tourists): Cần thông tin đa ngôn ngữ và hướng dẫn trực quan.

Food Reviewer/Giới trẻ: Cần thông tin chính xác về tọa độ, món "signature" của từng quán để trải nghiệm.

4. Kiến trúc & Hệ sinh thái công nghệ (Tech Stack & Ecosystem)
Hệ thống được thiết kế tối giản, tập trung vào client-side để đảm bảo tiêu chí Offline-first:

Frontend (Mobile App - .NET MAUI/C#): Giao diện tương tác chính. Tích hợp SQLite để lưu trữ cấu hình, nội dung text và xử lý dữ liệu offline tại chỗ. Toàn bộ dữ liệu quán ăn được đóng gói sẵn vào app.

Thanh toán In-App: Thư viện Plugin.InAppBilling kết nối với Apple App Store và Google Play.

Hệ sinh thái Zero-Cost:

Bản đồ: PMTiles (bản đồ vector offline gói gọn khu vực Quận 4), tệp kê khai (manifest), phông chữ cục bộ, ngăn chặn Path Traversal.

Âm thanh: Edge-TTS (tạo giọng nói), Deep-translator (dịch thuật đa ngôn ngữ).

Bản địa hóa (Localization): Cơ chế tải trước nội dung ngôn ngữ (poi-localizations) khả dụng 100% offline.

5. Yêu cầu chức năng & Luồng hoạt động (Functional Requirements & Flows)
5.1. Luồng Thanh toán & Mở khóa (Monetization & Paywall Flow)
Màn hình Onboarding: Tải app miễn phí, hiển thị 3-4 màn hình (Carousel) giới thiệu các tính năng hấp dẫn của tour Vĩnh Khánh (nghe audio tự động, bản đồ offline).

Trạm thu phí (Paywall): Khi khách nhấn "Bắt đầu tour", hiển thị màn hình thanh toán với thông tin giá vé rõ ràng (Ví dụ: "Mở khóa trọn bộ Tour Vĩnh Khánh chỉ với $1.99").

Thanh toán & Xác thực (IAP): Sử dụng cổng thanh toán của Apple/Google. Sau khi thanh toán thành công, hệ thống lưu hóa đơn (Receipt) vào thiết bị và cấp quyền mở khóa vĩnh viễn tính năng Tour.

Khôi phục mua hàng (Restore Purchase): Nút khôi phục dành cho khách hàng đã mua trước đó, dựa trên Apple ID/Google Account.

5.2. Luồng Khởi động & Đồng bộ (Offline Sync Flow)
Tải dữ liệu ban đầu: Ngay sau khi mở khóa thành công (có mạng), hệ thống tải gói dữ liệu (Bản đồ PMTiles, Text đa ngôn ngữ, Audio MP3 cơ bản) và lưu vào bộ nhớ cục bộ / SQLite.

Chế độ Ngoại tuyến: Người dùng hoàn toàn có thể ngắt mạng sau bước đồng bộ mà vẫn sử dụng được toàn bộ tính năng chính.

5.3. Luồng Khám phá (Discovery Flow)
Bản đồ tương tác: Xem bản đồ offline, quét các điểm đánh dấu (Marker) quán ăn trên phố Vĩnh Khánh.

Tìm kiếm & Lọc: Tìm món ăn/quán ăn, lọc theo danh mục (Ốc, Lẩu, Nướng, Tráng miệng).

Chi tiết POI (Point of Interest): Xem thông tin quán, hình ảnh, đặc sản nổi bật và menu đã được dịch sang ngôn ngữ của thiết bị.

5.4. Luồng Hàng rào địa lý & Âm thanh (Geofencing & Audio Flow)
Kích hoạt tự động: Ứng dụng chạy nền theo dõi tọa độ GPS. Khi du khách bước vào vùng Geofence (bán kính ~30m) của một quán và dừng lại (hoặc di chuyển chậm) > 3 giây.

Thuyết minh (Audio Guide): App tự động đánh thức màn hình (Wake-up) và phát Audio giới thiệu, kể câu chuyện về quán ăn/món ăn đó.

5.5. Luồng Tương tác cá nhân (Personalization Flow)
Đánh dấu yêu thích: Lưu quán ăn vào danh sách "Yêu thích" cá nhân trên thiết bị (Lưu cục bộ trong SQLite).

Nhật ký hành trình: Tự động check-in lưu lại danh sách những quán đã đi qua.

6. Yêu cầu phi chức năng (Non-Functional Requirements)
Tối ưu Pin (Battery Efficiency): Thuật toán đọc GPS ngầm phải được tối ưu (Throttle 5s/lần) để tránh cạn kiệt pin, tự động tắt định vị khi ra khỏi khu vực Quận 4.

Xử lý Ngoại tuyến (Offline Resilience): Màn hình không bao giờ được hiện báo lỗi mất mạng khi đang đi tour. Mọi luồng UI phải liên kết chặt chẽ với SQLite.

Độ trễ âm thanh (Audio Latency): Thời gian phản hồi từ lúc bước vào vùng nhận diện đến khi phát âm thanh phải dưới 2 giây.

Bảo mật thanh toán: Tuyệt đối không lưu trữ thông tin thẻ tín dụng của khách hàng trên hệ thống, phó thác hoàn toàn cho framework bảo mật của Apple/Google.

7. Giới hạn dự án (Out of Scope for MVP)
Tích hợp thanh toán In-App Purchase để bán app là Bắt buộc, nhưng KHÔNG làm tính năng thanh toán tiền mua đồ ăn hay đặt bàn tại các quán ăn trong app.

Không có tính năng mạng xã hội, bình luận công khai hay chat trực tuyến.

Không có cổng quản lý (Admin Dashboard) hay CMS động dành cho đối tác (Chủ quán). Dữ liệu được Hardcode hoặc đẩy qua API tĩnh từ đội ngũ phát triển.

KHÔNG yêu cầu người dùng phải tự đăng ký tài khoản (User System riêng của app) để giảm rào cản thao tác. Mọi định danh dựa vào Apple ID / Google Account của thiết bị.
