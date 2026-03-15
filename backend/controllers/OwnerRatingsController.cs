using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGU_FOOD_TOUR_PJ.data;
using SGU_FOOD_TOUR_PJ.models;

namespace SGU_FOOD_TOUR_PJ.controllers
{
    [Authorize(Roles = "SHOP_OWNER")]
    [ApiController]
    [Route("api/owner/ratings")]
    public class OwnerRatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OwnerRatingsController(ApplicationDbContext context) => _context = context;

        private Guid GetUserId() => Guid.Parse(User.FindFirst("userId")?.Value!);
        private async Task<Restaurant?> GetMyRestaurant() => 
            await _context.Restaurants.FirstOrDefaultAsync(r => r.owner_id == GetUserId());

        [HttpGet]
        public async Task<IActionResult> GetRatings()
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound();

            return Ok(new { message = "Chức năng đang chờ kết nối bảng Ratings trong CSDL." });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetRatingStats()
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound();

            return Ok(new { 
                average_stars = 4.5, 
                total_reviews = 120,
                five_stars = 80
            });
        }
    }
}