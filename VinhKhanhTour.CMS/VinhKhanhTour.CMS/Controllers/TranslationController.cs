using Microsoft.AspNetCore.Mvc;
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
        // 1. DANH SÁCH BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Sắp xếp theo ngôn ngữ rồi đến ID để dễ quản lý
            var translations = await _context.Translations
                                             .OrderBy(t => t.LanguageCode)
                                             .ThenByDescending(t => t.Id)
                                             .ToListAsync();
            return View(translations);
        }

        // ==========================================
        // 2. THÊM MỚI BẢN DỊCH
        // ==========================================
        public IActionResult Create()
        {
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
            return View(translation);
        }

        // ==========================================
        // 3. XEM CHI TIẾT
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var translation = await _context.Translations.FirstOrDefaultAsync(m => m.Id == id);
            if (translation == null) return NotFound();
            return View(translation);
        }

        // ==========================================
        // 4. SỬA BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null) return NotFound();
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
            return View(translation);
        }

        // ==========================================
        // 5. XÓA BẢN DỊCH
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var translation = await _context.Translations.FirstOrDefaultAsync(m => m.Id == id);
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