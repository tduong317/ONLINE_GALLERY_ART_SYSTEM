using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var banner = _context.Banners.ToList();

            var artwork = _context.Artworks
                .Include(a => a.Category) // Gọi luôn thông tin category
                .OrderBy(a => a.ArtworkId)
                .Take(6)
                .ToList();

            ViewBag.Banners = banner;
            return View(artwork);
        }

        public IActionResult Exhibition(int page=1) {
            var limit = 6;
            var exh = _context.Exhibitions.ToPagedList(page,limit);
            return View(exh);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
