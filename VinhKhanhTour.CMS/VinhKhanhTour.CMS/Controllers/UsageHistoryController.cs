using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;
using System.Linq;

namespace VinhKhanhTour.CMS.Controllers
{
    public class UsageHistoryController : Controller
    {
        private readonly AppDbContext _context;

        public UsageHistoryController(AppDbContext context)
        {
            _context = context;
        }

        // Chỉ cần hàm Xem danh sách, sắp xếp theo thời gian mới nhất
        public async Task<IActionResult> Index()
        {
            var history = await _context.UsageHistories
                                        .OrderByDescending(h => h.CreatedAt)
                                        .ToListAsync();
            return View(history);
        }
    }
}