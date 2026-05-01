using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System;
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

        [HttpGet]
        public async Task<IActionResult> GetPoiTranslations(string lang = "en-US")
        {
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

            if (!translations.Any()) return NotFound(new { message = $"Chưa có bản dịch cho ngôn ngữ: {lang}" });
            return Ok(translations);
        }

        // ==========================================
        // THÊM MỚI: API DỊCH TỰ ĐỘNG
        // ==========================================
        [HttpGet("AutoTranslate")]
        public async Task<IActionResult> AutoTranslate(int poiId, string targetLang)
        {
            var poi = await _context.Pois.FindAsync(poiId);
            if (poi == null) return NotFound();

            string langCode = targetLang.Split('-')[0];

            string translatedName = await TranslateWithGoogle(poi.Name, langCode);
            string translatedAddress = await TranslateWithGoogle(poi.Address, langCode);
            string translatedScript = await TranslateWithGoogle(poi.DisplayTtsScript, langCode);

            return Ok(new
            {
                name = translatedName,
                address = translatedAddress,
                script = translatedScript
            });
        }

        private async Task<string> TranslateWithGoogle(string text, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            try
            {
                using var client = new HttpClient();
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=vi&tl={targetLang}&dt=t&q={Uri.EscapeDataString(text)}";
                var response = await client.GetStringAsync(url);
                var jsonDocument = JsonDocument.Parse(response);
                return jsonDocument.RootElement[0][0][0].GetString() ?? text;
            }
            catch { return text; }
        }
    }
}