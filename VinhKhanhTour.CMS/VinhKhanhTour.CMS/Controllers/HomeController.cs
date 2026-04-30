using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;
using System.Linq; // BẮT BUỘC THÊM DÒNG NÀY ĐỂ DÙNG LỆNH .Where()

namespace VinhKhanhTour.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // THÊM MỚI: Đếm tổng số lượt mở App từ trước đến nay
            ViewBag.TotalAccess = await _context.UsageHistories
                .Where(h => h.ActionType == "Mở App")
                .CountAsync();

            // Các chỉ số khác giữ nguyên
            ViewBag.TotalPois = await _context.Pois.CountAsync();
            ViewBag.TotalAudios = await _context.Audios.CountAsync();
            ViewBag.TotalTranslations = await _context.Translations.CountAsync();

            return View();
        }
    }
}