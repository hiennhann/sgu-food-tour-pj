using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers.Api
{
    // Cấu hình đường dẫn API: https://localhost:xxxx/api/PoiApi
    [Route("api/[controller]")]
    [ApiController]
    public class PoiApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PoiApiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. API Lấy danh sách TẤT CẢ các quán ăn (chỉ lấy quán đang hoạt động)
        // GET: api/PoiApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poi>>> GetPois()
        {
            var pois = await _context.Pois
                                     .Where(p => p.IsActive == true)
                                     .OrderByDescending(p => p.Priority) // Ưu tiên quán nào có Priority cao lên trước
                                     .ToListAsync();

            return Ok(pois); // Tự động chuyển đổi danh sách thành chuỗi JSON
        }

        // 2. API Lấy chi tiết 1 quán theo ID (Dùng khi khách bấm vào quán trên App)
        // GET: api/PoiApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Poi>> GetPoi(int id)
        {
            var poi = await _context.Pois
                                    .Include(p => p.AudioTracks) // Lấy luôn danh sách file Audio của quán này
                                    .FirstOrDefaultAsync(p => p.Id == id);

            if (poi == null)
            {
                return NotFound(new { message = "Không tìm thấy quán ăn này!" });
            }

            return Ok(poi);
        }
    }
}