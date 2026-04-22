using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        // --- CÁCH 1: TRẠM KIỂM SOÁT TỪ ĐIỆN THOẠI GỌI LÊN ---
        [HttpPost("check")]
        public async Task<IActionResult> CheckDeviceStatus([FromBody] SubscriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceId))
                return BadRequest("DeviceId is required");

            var subscription = await _context.DeviceSubscriptions
                .FirstOrDefaultAsync(s => s.DeviceId == request.DeviceId);

            if (subscription == null)
            {
                subscription = new DeviceSubscription
                {
                    DeviceId = request.DeviceId,
                    FirstLaunchDate = DateTime.UtcNow,
                    IsPaid = false
                };
                _context.DeviceSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Trial", hoursRemaining = 24 });
            }

            if (subscription.IsPaid)
            {
                return Ok(new { status = "Premium" });
            }

            var timePassed = DateTime.UtcNow - subscription.FirstLaunchDate;
            if (timePassed.TotalHours < 24)
            {
                return Ok(new
                {
                    status = "Trial",
                    hoursRemaining = 24 - (int)timePassed.TotalHours
                });
            }

            return Ok(new { status = "Expired" });
        }

        // --- CÁCH 2: API DUYỆT CẤP QUYỀN APP DÀNH CHO ADMIN ---
        [HttpPost("approve/{deviceId}")]
        public async Task<IActionResult> ApproveDevice(string deviceId)
        {
            // Tìm thiết bị trong Database bằng DeviceId
            var subscription = await _context.DeviceSubscriptions
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId);

            if (subscription == null)
                return NotFound(new { message = "Không tìm thấy mã thiết bị này trong hệ thống! Vui lòng kiểm tra lại." });

            // Cập nhật trạng thái mở khóa thành công
            subscription.IsPaid = true;
            // Lưu lại vết thời gian duyệt thủ công
            subscription.PaymentReceipt = $"Duyệt thủ công qua API lúc {DateTime.UtcNow:dd/MM/yyyy HH:mm}";
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã cấp quyền truy cập Premium thành công cho thiết bị: {deviceId}" });
        }
    }
}