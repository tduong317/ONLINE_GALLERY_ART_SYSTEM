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

            int pageSize = 10;

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
            // Xử lý khi người dùng chọn ảnh qua elFinder
            if (!string.IsNullOrEmpty(art.ImageUrl))
            {
                try
                {
                    // Đường dẫn ảnh gốc (trong elFinder)
                    var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", art.ImageUrl.TrimStart('/'));

                    // Nếu file tồn tại, copy sang /images/artworks/
                    if (System.IO.File.Exists(sourcePath))
                    {
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/artworks");
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);

                        // Tạo tên file mới để tránh trùng
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                        var destPath = Path.Combine(uploads, fileName);

                        System.IO.File.Copy(sourcePath, destPath, true);

                        // Cập nhật đường dẫn mới trong DB
                        art.ImageUrl = "/images/artworks/" + fileName;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Copy image error: " + ex.Message);
                }
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
                return View(artwork);

            // Nếu người dùng đã chọn ảnh mới từ elFinder
            // Nếu người dùng chọn ảnh mới bằng elFinder
            if (!string.IsNullOrEmpty(artwork.ImageUrl) && artwork.ImageUrl != art.ImageUrl)
            {
                try
                {
                    var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", artwork.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(sourcePath))
                    {
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/artworks");
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                        var destPath = Path.Combine(uploads, fileName);

                        System.IO.File.Copy(sourcePath, destPath, true);

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(art.ImageUrl))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", art.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        art.ImageUrl = "/images/artworks/" + fileName;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Copy image error: " + ex.Message);
                }
            }


            // Cập nhật các trường khác
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
