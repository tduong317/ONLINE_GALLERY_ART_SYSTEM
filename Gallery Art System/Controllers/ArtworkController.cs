using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using X.PagedList.Extensions;


namespace Gallery_Art_System.Controllers
{
    public class ArtworkController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        public IActionResult Index(int? categoryId, int? UserId, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            int pageSize = 6;

            // Truy vấn artworks cơ bản
            var artworks = _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.User)
                .AsQueryable();

            // ======= Lọc theo danh mục =======
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
            }

            // ======= Lọc theo nghệ sĩ =======
            if (UserId.HasValue && UserId > 0)
            {
                artworks = artworks.Where(a => a.UserId == UserId);
            }

            // Phân trang & sắp xếp
            var pagedArtworks = artworks
                .OrderByDescending(a => a.CreatedAt)
                .ToPagedList(pageNumber, pageSize);

            // ======= Gửi dữ liệu cho View =======
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();

            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedUserId = UserId;

            return View(pagedArtworks);
        }



        [HttpGet]
        public IActionResult Create(int? categoryId, int? UserId)
        {
            // Lấy danh sách danh mục & nghệ sĩ cho dropdown
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();

            // Lưu lại ID đang được chọn (để hiển thị selected trong view)
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedUserId = UserId;

            // Tạo model Artwork mới, gán sẵn CategoryId và ArtistId
            var newArtwork = new Artwork
            {
                CategoryId = categoryId,
                UserId = UserId
            };

            return View(newArtwork);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Artwork art)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name");
                ViewBag.UserId = new SelectList(_context.Users, "UserId", "Username");
                return View(art);
            }


            // Xử lý upload ảnh
            if (art.ImageFile != null && art.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/artworks");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(art.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await art.ImageFile.CopyToAsync(stream);
                }

                art.ImageUrl = "/images/artworks/" + fileName;
            }

            art.CreatedAt = DateTime.Now;

            _context.Artworks.Add(art);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ct = _context.Artworks.Find(id);
            if (ct == null)
            {
                return NotFound($"Cannot find artwork with id: {id}");
            }

            // Gán ViewBag cho select
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();
            ViewBag.SelectedCategoryId = ct.CategoryId;

            return View(ct);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, Artwork artwork)
        {
            if (id != artwork.ArtworkId)
                return NotFound();

            var art = await _context.Artworks.FindAsync(id);
            if (art == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name");
                ViewBag.UserId = new SelectList(_context.Users, "UserId", "Username");
                return View(artwork);
            }


            // Nếu có ảnh mới
            if (artwork.ImageFile != null && artwork.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/artworks");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(artwork.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await artwork.ImageFile.CopyToAsync(stream);
                }

                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(art.ImageUrl))
                {
                    var oldPath = Path.Combine(uploads, art.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                art.ImageUrl = fileName;
            }

            // Cập nhật các trường còn lại
            art.Title = artwork.Title;
            art.Description = artwork.Description;
            art.Price = artwork.Price;
            art.CategoryId = artwork.CategoryId;
            art.SaleType = artwork.SaleType;
            art.Status = artwork.Status;
            art.Artist = artwork.Artist;
            art.UserId = artwork.UserId;
            art.CreatedAt = DateTime.Now;

            _context.Update(art);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Delete(int id)
        {
            var aw = _context.Artworks.Find(id);
            _context.Artworks.Remove(aw);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allArtworks = _context.Artworks.ToList();
            _context.Artworks.RemoveRange(allArtworks);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
