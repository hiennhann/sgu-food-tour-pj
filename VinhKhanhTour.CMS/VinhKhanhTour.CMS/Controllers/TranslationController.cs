using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        // DANH SÁCH BẢN DỊCH
        public async Task<IActionResult> Index()
        {
            var translations = await _context.Translations.ToListAsync();
            return View(translations);
        }

        // MỞ FORM THÊM MỚI
        public IActionResult Create()
        {
            return View();
        }

        // LƯU BẢN DỊCH MỚI
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
    }
}