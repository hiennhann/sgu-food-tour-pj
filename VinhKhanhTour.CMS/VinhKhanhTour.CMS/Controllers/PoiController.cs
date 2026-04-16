
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;

namespace VinhKhanhTour.CMS.Controllers
{
    public class PoiController : Controller
    {
        private readonly AppDbContext _context;

        public PoiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. MÀN HÌNH DANH SÁCH QUÁN ĂN (INDEX)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var pois = await _context.Pois.OrderByDescending(p => p.Id).ToListAsync();
            return View(pois);
        }

        // ==========================================
        // 2. MÀN HÌNH FORM THÊM MỚI (CREATE)
        // ==========================================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Poi poi, IFormFile? ImageFile)
        {
            // BƯỚC QUAN TRỌNG: Gỡ bỏ bắt lỗi tự động cho các trường không bắt buộc
            ModelState.Remove("ImageUrl");
            ModelState.Remove("DisplayTtsScript");
            ModelState.Remove("AudioTracks");

            // Gán giá trị mặc định an toàn nếu người dùng bỏ trống
            poi.Address ??= "";
            poi.Category ??= "Khác";
            poi.DisplayName ??= poi.Name;
            poi.DisplayTtsScript ??= ""; // Khách không nhập kịch bản thì lưu rỗng

            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh nếu có chọn
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(ImageFile.FileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;

                    var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                    var exactFilePath = Path.Combine(imageFolder, newFileName);
                    using (var stream = new FileStream(exactFilePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    poi.ImageUrl = newFileName;
                }
                else
                {
                    poi.ImageUrl = ""; // Không bắt buộc hình, cho rỗng luôn
                }

                _context.Pois.Add(poi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // MẸO DEBUG: Nếu vẫn báo lỗi, dòng này sẽ in thẳng tên Cột bị lỗi ra màn hình đen (Console)
            foreach (var modelStateKey in ViewData.ModelState.Keys)
            {
                var modelStateVal = ViewData.ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    Console.WriteLine($"[LỖI CREATE] Cột: {modelStateKey} - Lỗi: {error.ErrorMessage}");
                }
            }

            return View(poi);
        }

        // ==========================================
        // 3. MÀN HÌNH XEM CHI TIẾT (DETAILS)
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var poi = await _context.Pois.FirstOrDefaultAsync(m => m.Id == id);
            if (poi == null) return NotFound();

            return View(poi);
        }

        // ==========================================
        // 4. MÀN HÌNH CHỈNH SỬA (EDIT)
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var poi = await _context.Pois.FindAsync(id);
            if (poi == null) return NotFound();

            return View(poi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Poi poi, IFormFile? ImageFile)
        {
            if (id != poi.Id) return NotFound();

            var existingPoi = await _context.Pois.FindAsync(id);
            if (existingPoi == null) return NotFound();

            if (!string.IsNullOrEmpty(poi.Name)) existingPoi.Name = poi.Name;
            if (!string.IsNullOrEmpty(poi.DisplayName)) existingPoi.DisplayName = poi.DisplayName;

            existingPoi.DisplayTtsScript = string.IsNullOrEmpty(poi.DisplayTtsScript) ? "" : poi.DisplayTtsScript;

            if (!string.IsNullOrEmpty(poi.Address)) existingPoi.Address = poi.Address;
            if (!string.IsNullOrEmpty(poi.Category)) existingPoi.Category = poi.Category;

            if (poi.Latitude != 0) existingPoi.Latitude = poi.Latitude;
            if (poi.Longitude != 0) existingPoi.Longitude = poi.Longitude;
            if (poi.Radius != 0) existingPoi.Radius = poi.Radius;
            if (poi.Priority != 0) existingPoi.Priority = poi.Priority;

            existingPoi.IsActive = poi.IsActive;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(ImageFile.FileName);
                var newFileName = Guid.NewGuid().ToString() + fileExtension;
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);
                var exactFilePath = Path.Combine(imageFolder, newFileName);
                using (var stream = new FileStream(exactFilePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                existingPoi.ImageUrl = newFileName;
            }

            ModelState.Clear();

            try
            {
                _context.Update(existingPoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("LỖI SQL KHI LƯU: " + ex.Message);
                return View(existingPoi);
            }
        }

        // ==========================================
        // 5. MÀN HÌNH XÓA (DELETE)
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var poi = await _context.Pois.FirstOrDefaultAsync(m => m.Id == id);
            if (poi == null) return NotFound();

            return View(poi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poi = await _context.Pois.FindAsync(id);
            if (poi != null)
            {
                _context.Pois.Remove(poi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PoiExists(int id)
        {
            return _context.Pois.Any(e => e.Id == id);
        }
    }
}