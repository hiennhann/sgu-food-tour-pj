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
    [Route("api/owner/foods")] // Gán cứng URL
    public class OwnerFoodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OwnerFoodsController(ApplicationDbContext context) => _context = context;

        private Guid GetUserId() => Guid.Parse(User.FindFirst("userId")?.Value!);
        private async Task<Restaurant?> GetMyRestaurant() => 
            await _context.Restaurants.FirstOrDefaultAsync(r => r.owner_id == GetUserId());

        [HttpGet]
        public async Task<IActionResult> GetFoods()
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound("Bạn chưa có quán ăn nào.");

            var foods = await _context.Foods
                .Where(f => f.restaurant_id == restaurant.id && f.is_active)
                .ToListAsync();
            return Ok(foods);
        }

        [HttpPost]
        public async Task<IActionResult> AddFood([FromBody] FoodDto dto)
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound("Bạn chưa có quán ăn nào.");

            var food = new Food
            {
                id = Guid.NewGuid(),
                restaurant_id = restaurant.id,
                name = dto.Name,
                description = dto.Description,
                price = dto.Price,
                image_url = dto.ImageUrl,
                is_active = true
            };

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm món ăn thành công", food });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(Guid id, [FromBody] FoodDto dto)
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound();

            var food = await _context.Foods
                .FirstOrDefaultAsync(f => f.id == id && f.restaurant_id == restaurant.id && f.is_active);

            if (food == null) return NotFound("Không tìm thấy món ăn.");

            food.name = dto.Name;
            food.description = dto.Description;
            food.price = dto.Price;
            food.image_url = dto.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật món ăn thành công", food });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(Guid id)
        {
            var restaurant = await GetMyRestaurant();
            if (restaurant == null) return NotFound();

            var food = await _context.Foods
                .FirstOrDefaultAsync(f => f.id == id && f.restaurant_id == restaurant.id && f.is_active);

            if (food == null) return NotFound("Không tìm thấy món ăn.");

            food.is_active = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa món ăn khỏi thực đơn." });
        }
    }
}