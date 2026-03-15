using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGU_FOOD_TOUR_PJ.data;
using SGU_FOOD_TOUR_PJ.dto;
using SGU_FOOD_TOUR_PJ.models; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography; // Đã thêm thư viện chuẩn của .NET cho SHA256
using Microsoft.AspNetCore.Authorization;
namespace SGU_FOOD_TOUR_PJ.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Hàm hỗ trợ băm mật khẩu bằng SHA256 (Tự viết)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // Trả về chuỗi Base64
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginData)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.phone_number == loginData.PhoneNumber);

            if (user == null || !user.is_active)
                return Unauthorized("Tài khoản không tồn tại hoặc đã bị khóa.");

            // Băm mật khẩu người dùng nhập vào bằng SHA256 để so sánh với DB
            string hashedInput = HashPassword(loginData.Password);
            
            if (user.password_hash != hashedInput)
                return Unauthorized("Mật khẩu không chính xác.");

            var token = CreateToken(user);

            return Ok(new { 
                token = token, 
                role = user.role, 
                fullName = user.full_name 
            });
        }

        [HttpPost("register-shop")]
        public async Task<IActionResult> RegisterShop([FromBody] RegisterShopDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.phone_number == dto.PhoneNumber))
                return BadRequest("Số điện thoại này đã được đăng ký.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    id = Guid.NewGuid(), 
                    phone_number = dto.PhoneNumber,
                    password_hash = HashPassword(dto.Password), // Dùng hàm tự viết ở trên
                    full_name = dto.FullName,
                    role = "SHOP_OWNER",
                    is_active = true,
                    created_at = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var restaurant = new Restaurant
                {
                    id = Guid.NewGuid(),
                    owner_id = user.id,
                    name = dto.RestaurantName,
                    address = dto.Address,
                    latitude = dto.Latitude,
                    longitude = dto.Longitude,
                    image_url = dto.ImageUrl,
                    approval_status = "PENDING",
                    trigger_radius = 20, // Bán kính mặc định
                    is_active = true,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { message = "Đăng ký thành công. Vui lòng đợi Admin duyệt hồ sơ quán ăn." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Trong môi trường dev, có thể trả về ex.Message để dễ debug
                return StatusCode(500, $"Có lỗi xảy ra trong quá trình đăng ký: {ex.Message}");
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.full_name),
                new Claim(ClaimTypes.MobilePhone, user.phone_number),
                new Claim(ClaimTypes.Role, user.role),
                new Claim("userId", user.id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // API: Lấy thông tin tài khoản hiện tại (Dành cho Dashboard / App Profile)
        [Authorize] // Yêu cầu phải có Token hợp lệ (bất kể là Role gì)
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // 1. Bóc tách claim "userId" từ Token mà Frontend gửi lên
            var userIdClaim = User.FindFirst("userId")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });

            var userId = Guid.Parse(userIdClaim);

            // 2. Truy vấn Database để lấy thông tin mới nhất
            var user = await _context.Users
                .Where(u => u.id == userId && u.is_active)
                .Select(u => new 
                {
                    u.id,
                    u.phone_number,
                    u.full_name,
                    u.role,
                    u.created_at
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Tài khoản không tồn tại hoặc đã bị khóa." });

            // 3. Trả về dữ liệu (Tuyệt đối KHÔNG trả về password_hash)
            return Ok(user);
        }
    }
}