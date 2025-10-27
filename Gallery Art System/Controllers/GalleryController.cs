using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;

        public GalleryController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }


        public IActionResult Index(int? artistId, int? pageNumber)
        {
            int pageSize = 10;
            int pageIndex = pageNumber ?? 1;

            // Lấy danh sách User để đổ vào dropdown
            ViewBag.UserList = _context.Users
                .OrderBy(u => u.FullName)
                .ToList();

            // Gán lại ID tác giả đã chọn để giữ trạng thái trên giao diện
            ViewBag.SelectedArtistId = artistId;

            // Query cơ bản
            var galleriesQuery = _context.Galleries
                .Include(g => g.User)
                .OrderByDescending(g => g.GalleryId)
                .AsQueryable();

            // Nếu người dùng chọn tác giả thì lọc theo UserId
            if (artistId.HasValue)
            {
                galleriesQuery = galleriesQuery.Where(g => g.UserId == artistId.Value);
            }

            var galleries = galleriesQuery.ToPagedList(pageIndex, pageSize);

            return View(galleries);
        }


        // ========== DETAILS ==========
        public async Task<IActionResult> Details(int id)
        {
            var gallery = await _context.Galleries
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.GalleryId == id);

            if (gallery == null)
                return NotFound();

            return View(gallery);
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create(int? artistId)
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.SelectedArtistId = artistId; // Lưu lại tác giả đang chọn
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Gallery gallery)
        {
            // Bỏ validate lỗi navigation property User
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                gallery.CreatedAt = DateTime.Now;
                _context.Add(gallery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _context.Users.ToList();
            return View(gallery);
        }


        // ========== EDIT ==========
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null)
                return NotFound();

            ViewBag.Users = _context.Users.ToList();
            return View(gallery);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Gallery gallery)
        {
            ModelState.Remove("User");
            gallery.CreatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Update(gallery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.ToList();
            return View(gallery);
        }

        // ========== DELETE ==========
        public IActionResult Delete(int id)
        {
            var gallery = _context.Galleries.Find(id);
            if (gallery != null)
            {
                _context.Galleries.Remove(gallery);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            var allGallrey = _context.Galleries.ToList();
            _context.Galleries.RemoveRange(allGallrey);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}