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

        // Đã bổ sung tham số 'actionType' để nhận lệnh lọc từ giao diện
        public async Task<IActionResult> Index(string actionType)
        {
            var query = _context.UsageHistories.AsQueryable();

            // Nếu Admin có chọn loại hành động để lọc, tiến hành lọc dữ liệu
            if (!string.IsNullOrEmpty(actionType))
            {
                query = query.Where(h => h.ActionType == actionType);
                // Giữ lại lựa chọn cũ trên giao diện
                ViewBag.CurrentFilter = actionType;
            }

            // Lấy ra danh sách các ActionType duy nhất (để đổ vào Dropdown lọc)
            ViewBag.ActionTypes = await _context.UsageHistories
                                                .Select(h => h.ActionType)
                                                .Distinct()
                                                .ToListAsync();

            var history = await query.OrderByDescending(h => h.CreatedAt)
                                     .ToListAsync();

            return View(history);
        }
    }
}