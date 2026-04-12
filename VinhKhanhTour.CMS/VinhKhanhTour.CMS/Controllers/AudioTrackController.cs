using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Thư viện cần thiết cho hàm Any()
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class AudioTrackController : Controller
    {
        private readonly AppDbContext _context;

        public AudioTrackController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. MÀN HÌNH DANH SÁCH (INDEX)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Sắp xếp ID giảm dần để file audio mới thêm hiện lên đầu
            var audios = await _context.Audios.OrderByDescending(a => a.Id).ToListAsync();
            return View(audios);
        }

        // ==========================================
        // 2. MÀN HÌNH THÊM MỚI (CREATE)
        // ==========================================
        public IActionResult Create()
        {
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AudioTrack audio)
        {
            if (ModelState.IsValid)
            {
                _context.Audios.Add(audio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi, phải nạp lại danh sách quán để ô chọn không bị trống
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name");
            return View(audio);
        }

        // ==========================================
        // 3. MÀN HÌNH XEM CHI TIẾT (DETAILS)
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var audio = await _context.Audios.FirstOrDefaultAsync(m => m.Id == id);
            if (audio == null) return NotFound();

            return View(audio);
        }

        // ==========================================
        // 4. MÀN HÌNH CHỈNH SỬA (EDIT)
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var audio = await _context.Audios.FindAsync(id);
            if (audio == null) return NotFound();

            // Nạp danh sách quán và chọn sẵn quán hiện tại của Audio này
            ViewBag.PoiList = new SelectList(_context.Pois.OrderBy(p => p.Name), "Id", "Name", audio.PoiId);
            return View(audio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AudioTrack audio)
        {
            // Kiểm tra bảo mật: ID trên đường dẫn phải khớp với ID trong form
            if (id != audio.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(audio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AudioTrackExists(audio.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(audio);
        }

        // ==========================================
        // 5. MÀN HÌNH XÓA (DELETE)
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var audio = await _context.Audios.FirstOrDefaultAsync(m => m.Id == id);
            if (audio == null) return NotFound();

            return View(audio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var audio = await _context.Audios.FindAsync(id);
            if (audio != null)
            {
                _context.Audios.Remove(audio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Hàm hỗ trợ kiểm tra AudioTrack có tồn tại không
        private bool AudioTrackExists(int id)
        {
            return _context.Audios.Any(e => e.Id == id);
        }
    }
}