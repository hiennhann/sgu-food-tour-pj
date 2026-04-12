using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Đếm số lượng thực tế từ Database
            ViewBag.TotalTours = await _context.Tours.CountAsync();
            ViewBag.TotalPois = await _context.Pois.CountAsync();
            ViewBag.TotalAudios = await _context.Audios.CountAsync();
            ViewBag.TotalTranslations = await _context.Translations.CountAsync();

            return View();
        }
    }
}