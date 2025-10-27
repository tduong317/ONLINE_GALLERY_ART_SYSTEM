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
        public IActionResult Index(int? categoryId, int? artistId, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            int pageSize = 10;

            // Truy vấn artworks cơ bản
            var artworks = _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Artist)
                .AsQueryable();

            // ======= Lọc theo danh mục =======
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
            }

            // ======= Lọc theo nghệ sĩ =======
            if (artistId.HasValue && artistId > 0)
            {
                artworks = artworks.Where(a => a.ArtistId == artistId);
            }

            // Phân trang & sắp xếp
            var pagedArtworks = artworks
                .OrderByDescending(a => a.CreatedAt)
                .ToPagedList(pageNumber, pageSize);

            // ======= Gửi dữ liệu cho View =======
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();

            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedArtistId = artistId;

            return View(pagedArtworks);
        }



        [HttpGet]
        public IActionResult Create(int? categoryId, int? artistId)
        {
            // Lấy danh sách danh mục & nghệ sĩ cho dropdown
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();

            // Lưu lại ID đang được chọn (để hiển thị selected trong view)
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedArtistId = artistId;

            // Tạo model Artwork mới, gán sẵn CategoryId và ArtistId
            var newArtwork = new Artwork
            {
                CategoryId = categoryId,
                ArtistId = artistId
            };

            return View(newArtwork);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Artwork data)
        {
            if (data.Status == null)
                data.Status = false;
            if (!string.IsNullOrEmpty(data.ImageUrl))
            {
                // Đảm bảo đường dẫn đúng định dạng
                if (!data.ImageUrl.StartsWith("/"))
                {
                    data.ImageUrl = "/" + data.ImageUrl;
                }
            }
            data.CreatedAt = DateTime.Now;
            data.Artist = null;

            if (ModelState.IsValid)
            {
                _context.Artworks.Add(data);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.UserList = _context.Users.ToList();
            return View(data);
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Artwork data)
        {
            if (data.Status == null)
                data.Status = false;
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _context.Categories.ToList();
                ViewBag.UserList = _context.Users.ToList();
                return View(data);
            }

            var existingArtwork = _context.Artworks.AsNoTracking().FirstOrDefault(a => a.ArtworkId == id);
            if (existingArtwork == null)
                return NotFound();

            // Cập nhật các trường được sửa
            existingArtwork.Title = data.Title;
            existingArtwork.Description = data.Description;
            existingArtwork.Price = data.Price;
            existingArtwork.SaleType = data.SaleType;
            existingArtwork.CategoryId = data.CategoryId;
            existingArtwork.ArtistId = data.ArtistId;
            existingArtwork.Status = data.Status;

            // Xử lý ảnh
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0 && files[0].Length > 0)
            {
                var file = files[0];
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                existingArtwork.ImageUrl = "/" + fileName; // hoặc data.ImageUrl nếu dùng elfinder
            }

            _context.Artworks.Update(existingArtwork);
            _context.SaveChanges();

            return RedirectToAction("Index");
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
