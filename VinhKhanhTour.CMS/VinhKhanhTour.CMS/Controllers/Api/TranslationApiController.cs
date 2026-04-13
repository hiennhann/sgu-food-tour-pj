using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TranslationApiController(AppDbContext context)
        {
            _context = context;
        }

        // Lấy từ điển theo mã ngôn ngữ (VD: GET api/TranslationApi?lang=en-US)
        [HttpGet]
        public async Task<IActionResult> GetTranslations(string lang = "vi-VN")
        {
            // Chỉ lọc lấy các từ khóa của đúng ngôn ngữ người dùng đang chọn trên điện thoại
            var translations = await _context.Translations
                                             .Where(t => t.LanguageCode == lang)
                                             // Biến đổi thành dạng Dictionary { "Key": "Value" }
                                             .ToDictionaryAsync(t => t.KeyName, t => t.TranslatedValue);

            if (!translations.Any())
            {
                return NotFound(new { message = $"Chưa có bản dịch cho ngôn ngữ: {lang}" });
            }

            return Ok(translations);
            // Kết quả trả về sẽ đẹp như vầy: { "btn_scan": "Scan QR", "app_title": "Food Street" }
        }
    }
}