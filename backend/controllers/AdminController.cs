using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGU_FOOD_TOUR_PJ.data;
using SGU_FOOD_TOUR_PJ.dto;

namespace SGU_FOOD_TOUR_PJ.controllers
{
    [Authorize(Roles = "ADMIN")] // Chốt chặn an ninh: Chỉ Admin mới được vào
    [ApiController]
    [Route("api/admin")] 
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region 1. QUẢN LÝ CỬA HÀNG

        /// <summary>
        /// API lấy danh sách toàn bộ các quán ăn đã đăng ký trên hệ thống.
        /// Thường dùng để hiển thị bảng quản lý chung cho Admin (để phân trang, tìm kiếm).
        /// Dữ liệu được sắp xếp theo thời gian tạo mới nhất lên đầu.
        /// </summary>
        [HttpGet("restaurants")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new {
                    r.id, r.name, r.address, r.approval_status, r.is_active, r.created_at
                })
                .OrderByDescending(r => r.created_at)
                .ToListAsync();
            return Ok(restaurants);
        }

        /// <summary>
        /// API lấy danh sách các quán ăn đang ở trạng thái PENDING (chờ duyệt).
        /// Dùng để hiển thị trên màn hình "Phê duyệt đối tác" của Admin.
        /// </summary>
        [HttpGet("restaurants/pending")]
        public async Task<IActionResult> GetPending() 
        {
            var pending = await _context.Restaurants
                .Where(r => r.approval_status == "PENDING")
                .Select(r => new {
                    r.id, r.name, r.address, r.created_at, r.owner_id
                })
                .ToListAsync();
            return Ok(pending);
        }

        /// <summary>
        /// API duyệt một quán ăn hợp lệ.
        /// Sẽ thay đổi trạng thái của quán từ PENDING sang APPROVED, 
        /// cho phép quán này được hiển thị lên bản đồ của Khách du lịch.
        /// </summary>
        [HttpPut("restaurants/{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res == null) return NotFound("Không tìm thấy hồ sơ quán ăn.");

            res.approval_status = "APPROVED";
            res.updated_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Đã duyệt quán ăn lên hệ thống." });
        }

        /// <summary>
        /// API từ chối hồ sơ quán ăn (ví dụ: thông tin giả mạo, ảnh mờ).
        /// Đổi trạng thái sang REJECTED và bắt buộc phải kèm theo lý do để báo lại cho Chủ quán.
        /// </summary>
        [HttpPut("restaurants/{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDto dto)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res == null) return NotFound("Không tìm thấy hồ sơ quán ăn.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("Vui lòng nhập lý do từ chối.");

            res.approval_status = "REJECTED";
            res.rejection_reason = dto.Reason;
            res.updated_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Đã từ chối hồ sơ quán ăn." });
        }

        #endregion

        #region 2. QUẢN LÝ NGƯỜI DÙNG

        /// <summary>
        /// API lấy danh sách toàn bộ người dùng (cả Chủ quán và Khách du lịch).
        /// Đã được mã hóa/loại bỏ trường password_hash để đảm bảo an toàn bảo mật.
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new {
                    u.id, u.phone_number, u.full_name, u.role, u.is_active, u.created_at
                })
                .OrderByDescending(u => u.created_at)
                .ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// API thay đổi trạng thái hoạt động của một tài khoản (Khóa hoặc Mở khóa).
        /// Nếu người dùng vi phạm quy định, Admin gọi API này để vô hiệu hóa tài khoản (is_active = false).
        /// </summary>
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            // Đảo ngược trạng thái hiện tại
            user.is_active = !user.is_active; 
            await _context.SaveChangesAsync();

            string msg = user.is_active ? "Đã MỞ KHÓA tài khoản." : "Đã KHÓA tài khoản.";
            return Ok(new { message = msg, isActive = user.is_active });
        }

        #endregion

        #region 3. QUẢN LÝ NỘI DUNG

        /// <summary>
        /// API lấy danh sách các kịch bản/file ghi âm thuyết minh đa ngôn ngữ.
        /// Dùng để Admin kiểm duyệt nội dung xem có chứa ngôn từ nhạy cảm hay không trước khi phát cho khách.
        /// </summary>
        [HttpGet("narrations")]
        public async Task<IActionResult> GetAllNarrations()
        {
            var narrations = await _context.Narrations
                .Select(n => new {
                    n.id, n.restaurant_id, n.language_code, n.is_active, n.created_at
                })
                .ToListAsync();
            return Ok(narrations);
        }

        /// <summary>
        /// API xóa một đánh giá (Rating) từ khách du lịch.
        /// Dùng trong trường hợp đánh giá mang tính chất spam, chửi bới, cạnh tranh không lành mạnh.
        /// </summary>
        [HttpDelete("ratings/{id}")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            // Logic sẽ được mở khi bảng Ratings được cấu hình đầy đủ trong CSDL
            return Ok(new { message = "Chức năng đang chờ kết nối bảng Ratings trong CSDL." });
        }

        #endregion

        #region 4. THỐNG KÊ HỆ THỐNG

        /// <summary>
        /// API lấy các con số thống kê tổng quát của toàn hệ thống.
        /// Trả về các con số (tổng user, tổng quán, số quán đã duyệt...) để Frontend vẽ lên các thẻ (Cards) trên Dashboard.
        /// </summary>
        [HttpGet("stats/overview")]
        public async Task<IActionResult> GetOverviewStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalRestaurants = await _context.Restaurants.CountAsync();
            var totalApprovedRestaurants = await _context.Restaurants.CountAsync(r => r.approval_status == "APPROVED");
            var totalNarrations = await _context.Narrations.CountAsync();

            return Ok(new {
                TotalUsers = totalUsers,
                TotalRestaurants = totalRestaurants,
                ApprovedRestaurants = totalApprovedRestaurants,
                TotalNarrations = totalNarrations
            });
        }

        /// <summary>
        /// API trả về dữ liệu tọa độ (Latitude, Longitude) và độ đông đúc (Weight) của du khách.
        /// Được Mobile App hoặc Web Frontend sử dụng để vẽ Bản đồ nhiệt (Heatmap) thể hiện khu vực nào đang thu hút nhiều khách nhất.
        /// </summary>
        [HttpGet("stats/heatmap")]
        public async Task<IActionResult> GetHeatmap()
        {
            // Dữ liệu giả lập (Mock data) chờ thay thế bằng dữ liệu thật từ bảng VisitLogs
            var mockHeatmapData = new List<object>
            {
                new { Latitude = 10.7625, Longitude = 106.7020, Weight = 100 }, 
                new { Latitude = 10.7630, Longitude = 106.7015, Weight = 50 },
                new { Latitude = 10.7618, Longitude = 106.7030, Weight = 80 }
            };

            return Ok(mockHeatmapData);
        }

        #endregion
    }
}