using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckInApiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. API: Lưu lịch sử Check-in
        [HttpPost]
        public async Task<IActionResult> CreateCheckIn([FromBody] CheckInRecord record)
        {
            _context.CheckInRecords.Add(record);
            await _context.SaveChangesAsync();
            return Ok(record);
        }

        // 2. API: Lấy toàn bộ nhật ký của 1 người dùng (Kèm tên quán và ảnh)
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMyJourney(int userId)
        {
            // Kết hợp bảng CheckIn và bảng Poi để lấy ra tên quán
            var history = await _context.CheckInRecords
                .Where(c => c.UserId == userId)
                .Join(_context.Pois,
                      checkin => checkin.PoiId,
                      poi => poi.Id,
                      (checkin, poi) => new
                      {
                          checkin.Id,
                          checkin.CheckInTime,
                          checkin.Note,
                          PoiName = poi.Name,
                          PoiImage = poi.ImageUrl
                      })
                .OrderByDescending(c => c.CheckInTime)
                .ToListAsync();

            return Ok(history);
        }
    }
}