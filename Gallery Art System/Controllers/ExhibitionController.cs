using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gallery_Art_System.Controllers
{
    public class ExhibitionController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;
        private readonly IWebHostEnvironment _env;

        public ExhibitionController(ONLINE_GALLERY_ART_SYSTEMContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Exhibition
        public IActionResult Index(int? pageNumber, string? Name)
        {
            int pageSize = 10;
            int pageIndex = pageNumber ?? 1;

            // Truy vấn từ bảng Exhibitions
            var exhibitions = _context.Exhibitions.AsQueryable();

            // Lọc theo tên Exhibition
            if (!string.IsNullOrEmpty(Name))
            {
                exhibitions = exhibitions.Where(e => e.Name.Contains(Name));
            }

            // Giữ lại giá trị tìm kiếm để hiển thị lại trên view
            ViewBag.Name = Name;

            // Phân trang và sắp xếp
            var pagedExhibitions = exhibitions
                .OrderByDescending(e => e.ExhibitionId)
                .ToPagedList(pageIndex, pageSize);

            return View(pagedExhibitions);
        }


        public IActionResult Create()
        {
            return View(new Exhibition());
        }

        [HttpPost]
        public IActionResult Create(Exhibition data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images//exhibitions", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.ImageUrl = FileName;
                }
            }
            _context.Exhibitions.Add(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Exhibition/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ex = _context.Exhibitions.Find(id);
            if (ex == null)
            {
                return NotFound($"Can not fint the contact with id: {id}");
            }
            return View(ex);
        }

        [HttpPost]
        public IActionResult Edit(Exhibition data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images/exhibitions", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.ImageUrl = FileName;
                }
            }
            _context.Exhibitions.Update(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var exhibition = _context.Exhibitions.Find(id);
            if (exhibition != null)
            {
                _context.Exhibitions.Remove(exhibition);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        
    }
}