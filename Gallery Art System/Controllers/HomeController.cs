using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        public IActionResult Artwork(int page = 1, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            int limit = 6;

            // Lấy danh sách category để hiển thị ở sidebar
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            // Lưu lại giá trị filter để hiển thị lại trên giao diện
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Lấy toàn bộ artworks (chưa phân trang)
            var artworks = _context.Artworks
                .Include(a => a.Category)
                .Where(a => a.SaleType == "For Sale")
                .AsQueryable();

            // Lọc theo category
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
            }

            // Lọc theo giá (nếu có)
            if (minPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price <= maxPrice.Value);
            }

            // Phân trang
            var pagedArtworks = artworks.ToPagedList(page, limit);

            return View(pagedArtworks);
        }

        public IActionResult Auction( int? categoryId) {
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;
            var auctions = _context.Auctions
                .Include(a=>a.Artwork)
                .ToList();
            foreach (var auc in auctions)
            {
                if (DateTime.Now < auc.StartTime)
                    auc.Status = "Upcoming";
                else if (DateTime.Now >= auc.StartTime && DateTime.Now <= auc.EndTime)
                    auc.Status = "Active";
                else
                    auc.Status = "Closed";
            }
            _context.SaveChanges();

            return View(auctions);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Artwork)
                .Include(a => a.Bids)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.AuctionId == id);

            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
