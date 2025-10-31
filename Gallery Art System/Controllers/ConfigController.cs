using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using X.PagedList.Extensions;
namespace Gallery_Art_System.Controllers
{
    public class ConfigController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;

        // 👉 Nhận DbContext qua constructor (Dependency Injection)
        public ConfigController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(int pageNumber = 1)
        {
            var pageSize = 6;
            var pageList = _context.Configs.OrderBy(cf => cf.Id).ToPagedList(pageNumber, pageSize);
            return View(pageList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Config data)
        {
            // Gán string.Empty cho tất cả các property kiểu string nếu đang là chuỗi rỗng
            foreach (PropertyInfo prop in data.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var value = prop.GetValue(data) as string;
                if (value == "")
                {
                    prop.SetValue(data, string.Empty);
                }
            }
            _context.Configs.Add(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Config obj = _context.Configs.FirstOrDefault(c => c.Id == 1);
            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(Config data)
        {
            // Lấy bản ghi cũ
            var oldConfig = _context.Configs.AsNoTracking().FirstOrDefault(c => c.Id == data.Id);
            if (oldConfig == null)
                return NotFound();

            // Nếu Image rỗng thì giữ ảnh cũ
            if (string.IsNullOrEmpty(data.Image))
            {
                data.Image = oldConfig.Image;
            }

            if (ModelState.IsValid)
            {
                _context.Configs.Update(data);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { id = data.Id });
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cf = _context.Configs.Find(id);
            if (cf == null)
            {
                return NotFound();
            }
            else
            {
                _context.Remove(cf);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
