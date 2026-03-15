using Microsoft.AspNetCore.Authorization; // Thư viện cho [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thư viện cho .ToListAsync()
using SGU_FOOD_TOUR_PJ.data;
using SGU_FOOD_TOUR_PJ.dto; // Thư viện để dùng RejectDto

namespace SGU_FOOD_TOUR_PJ.controllers
{
    [Authorize(Roles = "ADMIN")] // Chỉ Admin mới được truy cập các API trong class này
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API 1: Lấy danh sách quán đang chờ duyệt
        [HttpGet("pending-restaurants")]
        public async Task<IActionResult> GetPending() 
        {
            var pending = await _context.Restaurants
                .Where(r => r.approval_status == "PENDING")
                .ToListAsync();
            
            return Ok(pending);
        }

        // API 2: Duyệt quán (Đổi trạng thái thành APPROVED)
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res == null) 
                return NotFound(new { message = "Không tìm thấy hồ sơ quán ăn." });

            res.approval_status = "APPROVED";
            res.updated_at = DateTime.UtcNow; // Cập nhật lại thời gian đồng bộ
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Đã duyệt quán ăn này lên hệ thống thành công." });
        }

        // API 3: Từ chối quán (Đổi trạng thái thành REJECTED và lưu lý do)
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDto dto)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res == null) 
                return NotFound(new { message = "Không tìm thấy hồ sơ quán ăn." });

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest(new { message = "Vui lòng nhập lý do từ chối." });

            res.approval_status = "REJECTED";
            res.rejection_reason = dto.Reason;
            res.updated_at = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Đã từ chối hồ sơ quán ăn này." });
        }
    }
}