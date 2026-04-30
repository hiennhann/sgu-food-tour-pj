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

        // Lấy danh sách bản dịch Quán ăn (POI) theo mã ngôn ngữ 
        // VD: GET api/TranslationApi?lang=ko-KR
        [HttpGet]
        public async Task<IActionResult> GetPoiTranslations(string lang = "en-US")
        {
            // Tìm tất cả các bản dịch thuộc về ngôn ngữ được truyền vào
            var translations = await _context.Translations
                                             .Where(t => t.LanguageCode == lang)
                                             .Select(t => new
                                             {
                                                 PoiId = t.PoiId,
                                                 TranslatedName = t.Name,
                                                 TranslatedAddress = t.Address,
                                                 TranslatedTtsScript = t.TtsScript
                                             })
                                             .ToListAsync();

            if (!translations.Any())
            {
                return NotFound(new { message = $"Chưa có bản dịch cho ngôn ngữ: {lang}" });
            }

            return Ok(translations);
            // Kết quả trả về sẽ là một danh sách rõ ràng:
            // [ { "poiId": 1, "translatedName": "Vu Snail", "translatedAddress": "...", "translatedTtsScript": "..." } ]
        }
    }
}