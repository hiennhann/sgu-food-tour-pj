using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;
using System.Linq;
using System;

namespace VinhKhanhTour.CMS.Controllers
{
    public class UsageHistoryController : Controller
    {
        private readonly AppDbContext _context;

        public UsageHistoryController(AppDbContext context)
        {
            _context = context;
        }

        // --- GIỮ NGUYÊN CHỨC NĂNG LỌC CŨ ---
        public async Task<IActionResult> Index(string actionType)
        {
            var query = _context.UsageHistories.AsQueryable();

            if (!string.IsNullOrEmpty(actionType))
            {
                query = query.Where(h => h.ActionType == actionType);
                ViewBag.CurrentFilter = actionType;
            }

            ViewBag.ActionTypes = await _context.UsageHistories
                                                .Select(h => h.ActionType)
                                                .Distinct()
                                                .ToListAsync();

            var history = await query.OrderByDescending(h => h.CreatedAt)
                                     .ToListAsync();

            return View(history);
        }

        // --- THÊM PHẦN TIẾP NHẬN TỪ APP (API) ---
        [HttpPost]
        [Route("api/UsageHistory/LogAccess")]
        public async Task<IActionResult> LogAccess([FromBody] AppLogRequest request)
        {
            if (request == null) return BadRequest();

            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                var history = new UsageHistory
                {
                    DeviceId = request.DeviceId,
                    IpAddress = ip ?? "N/A",
                    ActionType = "Mở App",
                    Details = $"Thiết bị {request.DeviceModel} vừa truy cập ứng dụng.",

                    // ĐỔI Now THÀNH UtcNow
                    CreatedAt = DateTime.UtcNow
                };

                _context.UsageHistories.Add(history);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }

    public class AppLogRequest
    {
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
    }
}