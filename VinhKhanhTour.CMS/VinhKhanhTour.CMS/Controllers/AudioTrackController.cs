using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var audios = await _context.Audios.ToListAsync();
            return View(audios);
        }

        public IActionResult Create()
        {
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
            return View(audio);
        }
    }
}