using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class PoiController : Controller
    {
        private readonly AppDbContext _context;

        // Bơm Database vào Controller
        public PoiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. MÀN HÌNH DANH SÁCH QUÁN ĂN
        public async Task<IActionResult> Index()
        {
            var pois = await _context.Pois.ToListAsync();
            return View(pois);
        }

        // 2. MÀN HÌNH FORM THÊM MỚI (Mở form)
        public IActionResult Create()
        {
            return View();
        }

        // 3. XỬ LÝ LƯU DỮ LIỆU KHI BẤM NÚT "LƯU"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Poi poi)
        {
            if (ModelState.IsValid)
            {
                _context.Pois.Add(poi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Lưu xong thì quay về trang danh sách
            }
            return View(poi); // Báo lỗi nếu nhập thiếu
        }
    }
}