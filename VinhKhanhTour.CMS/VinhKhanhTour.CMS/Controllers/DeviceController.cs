using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class DeviceController : Controller
    {
        private readonly AppDbContext _context;

        public DeviceController(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách thiết bị hiển thị lên màn hình (ưu tiên máy tải gần nhất lên đầu)
        public async Task<IActionResult> Index()
        {
            var devices = await _context.DeviceSubscriptions
                                .OrderByDescending(d => d.FirstLaunchDate)
                                .ToListAsync();
            return View(devices);
        }

        // Nút nhấn: CẤP QUYỀN / DUYỆT THANH TOÁN
        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            var device = await _context.DeviceSubscriptions.FindAsync(id);
            if (device != null)
            {
                device.IsPaid = true;
                device.PaymentReceipt = $"Duyệt trên Web lúc: {DateTime.Now:dd/MM/yyyy HH:mm}";
                device.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã mở khóa VIP vĩnh viễn cho thiết bị chứa ID: {device.DeviceId}";
            }
            return RedirectToAction(nameof(Index));
        }

        // Nút nhấn: THU HỒI / HỦY QUYỀN
        [HttpPost]
        public async Task<IActionResult> Revoke(Guid id)
        {
            var device = await _context.DeviceSubscriptions.FindAsync(id);
            if (device != null)
            {
                device.IsPaid = false;
                device.PaymentReceipt = $"Đã bị hệ thống huỷ quyền lúc: {DateTime.Now:dd/MM/yyyy HH:mm}";
                device.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã khóa lại thiết bị mang ID: {device.DeviceId}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}