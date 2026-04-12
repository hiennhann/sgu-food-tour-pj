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
            // Lấy danh sách quán, sắp xếp theo ID giảm dần (quán mới thêm lên đầu)
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
            //// Bỏ qua kiểm tra lỗi của các trường không có trên Form
            //ModelState.Remove("ImageUrl");
            //ModelState.Remove("Address");
            //ModelState.Remove("Category");
            //ModelState.Remove("DisplayName");

            //// Gán giá trị mặc định (chuỗi rỗng) để Database không bị lỗi "NOT NULL"
            //poi.Address ??= "";
            //poi.Category ??= "Chưa phân loại";
            //poi.DisplayName ??= poi.Name; // L

            if (ModelState.IsValid)
            {
                // Xử lý lưu file ảnh
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

                    poi.ImageUrl = newFileName; // Lưu tên file vào DB
                }
                else
                {
                    poi.ImageUrl = ""; // Nếu không up ảnh thì để trống
                }

                _context.Pois.Add(poi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // In lỗi ra Console nếu vẫn còn lỗi Validate
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var err in errors)
            {
                Console.WriteLine("LỖI VALIDATE: " + err);
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

            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu up ảnh mới thì xử lý lưu ảnh
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
                    // Nếu không up ảnh mới, thuộc tính ImageUrl cũ (được giữ trong thẻ hidden) sẽ tự được dùng lại

                    _context.Update(poi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoiExists(poi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(poi);
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