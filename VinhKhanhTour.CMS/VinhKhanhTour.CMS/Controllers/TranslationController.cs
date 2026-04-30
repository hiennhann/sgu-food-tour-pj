using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class TranslationController : Controller
    {
        private readonly AppDbContext _context;

        public TranslationController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DANH SÁCH BẢN DỊCH (POI TRANSLATIONS)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // .Include(t => t.Poi) để lấy được thông tên quán gốc hiển thị ra bảng
            var translations = await _context.Translations
                                             .Include(t => t.Poi)
                                             .OrderBy(t => t.LanguageCode)
                                             .ThenBy(t => t.Poi.Name)
                                             .ToListAsync();
            return View(translations);
        }

        // ==========================================
        // 2. THÊM MỚI BẢN DỊCH
        // ==========================================
        public IActionResult Create()
        {
            // Lấy danh sách POI đang hoạt động để Admin chọn dịch
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Translation translation)
        {
            if (ModelState.IsValid)
            {
                _context.Translations.Add(translation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi (thiếu dữ liệu), phải nạp lại danh sách POI để Dropdown không bị trống
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name", translation.PoiId);
            return View(translation);
        }

        // ==========================================
        // 3. SỬA BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var translation = await _context.Translations.FindAsync(id);
            if (translation == null) return NotFound();

            // Nạp danh sách POI và chọn sẵn cái POI hiện tại
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name", translation.PoiId);
            return View(translation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Translation translation)
        {
            if (id != translation.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(translation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TranslationExists(translation.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name", translation.PoiId);
            return View(translation);
        }

        // ==========================================
        // 4. CHI TIẾT BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var translation = await _context.Translations
                                             .Include(t => t.Poi)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (translation == null) return NotFound();
            return View(translation);
        }

        // ==========================================
        // 5. XÓA BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var translation = await _context.Translations
                                             .Include(t => t.Poi)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (translation == null) return NotFound();
            return View(translation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation != null)
            {
                _context.Translations.Remove(translation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TranslationExists(int id)
        {
            return _context.Translations.Any(e => e.Id == id);
        }
    }
}