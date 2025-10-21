using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext adm = new ONLINE_GALLERY_ART_SYSTEMContext();
        public IActionResult Index(int pageNumber = 1)
        {
            var pageSize = 10;
            var pageList = adm.Admins.OrderBy(adm => adm.Id).ToPagedList(pageNumber, pageSize);
            return View(pageList);
        }
        public IActionResult AdminIndex()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Admin data)
        {
            if (ModelState.IsValid)
            {
                // Mã hoá mật khẩu
                data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);

                adm.Admins.Add(data);
                adm.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Edit(int id)
        {
            var user = adm.Admins.Find(id);
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(Admin data)
        {
            if (ModelState.IsValid)
            {
                // Lấy user cũ từ DB
                var userInDb = adm.Admins.Find(data.Id);
                if (userInDb == null)
                {
                    return NotFound();
                }

                // Nếu nhập mật khẩu mới thì hash và cập nhật
                if (!string.IsNullOrEmpty(data.Password))
                {
                    userInDb.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
                }

                // Cập nhật các thông tin khác
                userInDb.UserName = data.UserName;
                userInDb.Name = data.Name;
                // Nếu còn field nào khác thì thêm ở đây

                adm.Admins.Update(userInDb);
                adm.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var user = adm.Admins.Find(id);
            adm.Admins.Remove(user);
            adm.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allUsers = adm.Admins.ToList();
            adm.Admins.RemoveRange(allUsers);
            adm.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
