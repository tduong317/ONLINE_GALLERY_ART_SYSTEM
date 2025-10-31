using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using X.PagedList.Extensions;

namespace StudyDocumentPlatform.Controllers
{
    public class BannerController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext bns = new ONLINE_GALLERY_ART_SYSTEMContext();
        [HttpGet]
        public IActionResult Index(int pageNumber = 1)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var pageSize = 6;
            var pageList = bns.Banners.OrderBy(c => c.Id).ToPagedList(pageNumber, pageSize);
            return View(pageList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Banner data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images//banners", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.Image = FileName;
                }
            }
            bns.Banners.Add(data);
            bns.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ct = bns.Banners.Find(id);
            if (ct == null)
            {
                return NotFound($"Can not fint the contact with id: {id}");
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Edit(Banner data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images//banners", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.Image = FileName;
                }
            }
            bns.Banners.Update(data);
            bns.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var bn = bns.Banners.Find(id);
            bns.Banners.Remove(bn);
            bns.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allBanners = bns.Banners.ToList();
            bns.Banners.RemoveRange(allBanners);
            bns.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
