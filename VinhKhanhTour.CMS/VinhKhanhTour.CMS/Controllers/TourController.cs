using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class TourController : Controller
    {
        private readonly AppDbContext _context;

        public TourController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DANH SÁCH TOUR
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var tours = await _context.Tours.OrderByDescending(t => t.Id).ToListAsync();
            return View(tours);
        }

        // ==========================================
        // 2. THÊM MỚI TOUR
        // ==========================================
        public IActionResult Create()
        {
            // Lấy toàn bộ danh sách quán ăn để Admin chọn
            ViewBag.PoiList = _context.Pois.OrderBy(p => p.Name).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tour tour, IFormFile? ImageFile)
        {
            ModelState.Remove("CoverImageUrl"); // Bỏ qua validate vì tự tạo tên file

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(ImageFile.FileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;
                    var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tours");
                    if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                    var exactFilePath = Path.Combine(imageFolder, newFileName);
                    using (var stream = new FileStream(exactFilePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    tour.CoverImageUrl = newFileName;
                }
                else
                {
                    tour.CoverImageUrl = "";
                }

                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // ==========================================
        // 3. XEM CHI TIẾT
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        // ==========================================
        // 4. SỬA TOUR
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tour tour, IFormFile? ImageFile)
        {
            if (id != tour.Id) return NotFound();
            ModelState.Remove("CoverImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(ImageFile.FileName);
                        var newFileName = Guid.NewGuid().ToString() + fileExtension;
                        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tours");
                        if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                        var exactFilePath = Path.Combine(imageFolder, newFileName);
                        using (var stream = new FileStream(exactFilePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        tour.CoverImageUrl = newFileName;
                    }
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // ==========================================
        // 5. XÓA TOUR
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }
    }
}