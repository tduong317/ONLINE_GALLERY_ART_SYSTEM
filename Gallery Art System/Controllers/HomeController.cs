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
            ViewBag.Banners = banner;
            List<Config> cf = _context.Configs.Where(cf => cf.Id == 1).ToList();
            ViewBag.Config = cf;
            List<Exhibition> ex = _context.Exhibitions
                .OrderByDescending(e => e.StartDate)
                .Take(3)
                .ToList();
            ViewBag.Exhibitions = ex;
            var artwork = _context.Artworks
                .Include(a => a.Category) // Gọi luôn thông tin category
                .OrderBy(a => a.ArtworkId)
                .Take(6)
                .ToList();
            
            return View(artwork);
        }

        public IActionResult Exhibition(string? query, int page = 1)
        {
            int limit = 3;

            // Bắt đầu truy vấn Exhibition
            var exhibitions = _context.Exhibitions.AsQueryable();

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(query))
            {
                exhibitions = exhibitions.Where(e =>
                    e.Name.Contains(query) ||
                    (e.Location != null && e.Location.Contains(query)) ||
                    (e.Description != null && e.Description.Contains(query))
                );

                ViewBag.Query = query; // Giữ lại từ khóa để hiển thị trong ô tìm kiếm
            }

            // Thực hiện phân trang
            var pagedList = exhibitions
                .OrderByDescending(e => e.StartDate)
                .ToPagedList(page, limit);

            return View(pagedList);
        }
        public IActionResult Artwork(int page = 1, int? categoryId = null)
        {
            int limit = 6;

            // Lấy danh sách category để hiển thị ở sidebar
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;
            // Lấy toàn bộ artworks (chưa phân trang)
            var artworks = _context.Artworks
               .Include(a => a.Category)
               .Where(a => a.SaleType == "For Sale") // 👈 lọc ngay từ đầu
               .AsQueryable();

            // Nếu có categoryId thì lọc theo category đó
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
                ViewBag.SelectedCategoryId = categoryId; // để biết đang chọn category nào
            }

            // Cuối cùng mới phân trang
            var pagedArtworks = artworks.ToPagedList(page, limit);

            return View(pagedArtworks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
