using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TourApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TourApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            // Lấy danh sách Tour mới nhất đưa lên đầu
            var tours = await _context.Tours
                                      .OrderByDescending(t => t.Id)
                                      .ToListAsync();
            return Ok(tours);
        }

        // GET: api/TourApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);

            if (tour == null)
            {
                return NotFound(new { message = "Không tìm thấy Tour này!" });
            }

            return Ok(tour);
        }
    }
}