using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class GalleryArtworkController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext ctx;

        public GalleryArtworkController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            ctx = context;
        }

        // ========== INDEX ==========
        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 6; 
            int pageIndex = pageNumber ?? 1; 

            var list = ctx.GalleryArtworks
                          .Include(g => g.Gallery)
                          .Include(a => a.Artwork)
                          .OrderByDescending(x => x.AddedAt)
                          .ToPagedList(pageIndex, pageSize);

            return View(list); 
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create(int? galleryId, int? artworkId)
        {
            ViewBag.Galleries = ctx.Galleries.ToList();
            ViewBag.Artworks = ctx.Artworks.ToList();
            ViewBag.SelectedGalleryId = galleryId;
            ViewBag.SelectedArtworkId = artworkId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GalleryArtwork model)
        {
            // Bỏ validation lỗi navigation properties
            ModelState.Remove("Gallery");
            ModelState.Remove("Artwork");

            if (ModelState.IsValid)
            {
                model.AddedAt = DateTime.Now;
                ctx.GalleryArtworks.Add(model);
                ctx.SaveChanges();

                // ✅ Quay về Index sau khi lưu
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi thì load lại dữ liệu dropdown
            ViewBag.Galleries = ctx.Galleries.ToList();
            ViewBag.Artworks = ctx.Artworks.ToList();
            return View(model);
        }


        // ========== EDIT ==========
        [HttpGet]
        public IActionResult Edit(int galleryId, int artworkId)
        {
            var item = ctx.GalleryArtworks
                          .FirstOrDefault(x => x.GalleryId == galleryId && x.ArtworkId == artworkId);
            if (item == null) return NotFound();

            ViewBag.Galleries = ctx.Galleries.ToList();
            ViewBag.Artworks = ctx.Artworks.ToList();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GalleryArtwork model, int originalGalleryId, int originalArtworkId)
        {
            ModelState.Remove("Gallery");
            ModelState.Remove("Artwork");

            if (ModelState.IsValid)
            {
                var oldItem = ctx.GalleryArtworks
                    .FirstOrDefault(x => x.GalleryId == originalGalleryId && x.ArtworkId == originalArtworkId);

                if (oldItem == null)
                    return NotFound();

                // Xóa bản ghi cũ
                ctx.GalleryArtworks.Remove(oldItem);

                // Tạo bản ghi mới với khóa mới
                model.AddedAt = DateTime.Now;
                ctx.GalleryArtworks.Add(model);

                ctx.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Galleries = ctx.Galleries.ToList();
            ViewBag.Artworks = ctx.Artworks.ToList();
            return View(model);
        }


        // ========== DELETE ==========
        public IActionResult Delete(int galleryId, int artworkId)
        {
            var item = ctx.GalleryArtworks
                          .FirstOrDefault(x => x.GalleryId == galleryId && x.ArtworkId == artworkId);
            if (item != null)
            {
                ctx.GalleryArtworks.Remove(item);
                ctx.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }       
    }
}