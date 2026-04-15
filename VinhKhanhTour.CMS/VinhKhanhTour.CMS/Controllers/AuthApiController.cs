using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthApiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. API Kiểm tra email trùng
        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool exists = await _context.UserAccounts.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            return Ok(exists);
        }

        // 2. API Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAccount user)
        {
            if (await _context.UserAccounts.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower()))
                return BadRequest("Email đã tồn tại");

            user.Email = user.Email.ToLower();
            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // 3. API Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAccount loginData)
        {
            // 1. Kiểm tra email có tồn tại không
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginData.Email.ToLower());

            if (user == null)
                return NotFound("Email chưa được đăng ký");

            // 2. Kiểm tra mật khẩu
            if (user.Password != loginData.Password)
                return Unauthorized("Sai mật khẩu");

            // 3. Thành công
            return Ok(user);
        }
    }
}