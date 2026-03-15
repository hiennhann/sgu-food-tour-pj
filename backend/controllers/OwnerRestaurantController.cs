using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGU_FOOD_TOUR_PJ.data;
using SGU_FOOD_TOUR_PJ.dto;
using SGU_FOOD_TOUR_PJ.models;

namespace SGU_FOOD_TOUR_PJ.controllers
{
    [Authorize(Roles = "SHOP_OWNER")]
    [ApiController]
    [Route("api/owner/restaurant")] // Gán cứng URL
    public class OwnerRestaurantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OwnerRestaurantController(ApplicationDbContext context) => _context = context;

        private Guid GetUserId() => Guid.Parse(User.FindFirst("userId")?.Value!);
        private async Task<Restaurant?> GetMyRestaurant() => 
            await _context.Restaurants.FirstOrDefaultAsync(r => r.owner_id == GetUserId());

        [HttpGet]
        public async Task<IActionResult> GetRestaurant()
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound("Bạn chưa có quán ăn nào.");
            return Ok(restaurant);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantDto dto)
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound("Bạn chưa có quán ăn nào.");

            restaurant.name = dto.Name;
            restaurant.address = dto.Address;
            restaurant.latitude = dto.Latitude;
            restaurant.longitude = dto.Longitude;
            restaurant.description = dto.Description;
            restaurant.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin quán thành công", restaurant });
        }
    }
}