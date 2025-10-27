using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class UserController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext us = new ONLINE_GALLERY_ART_SYSTEMContext();
        public IActionResult Index(string? FullName, int pageNumber = 1)
        {
            int pageSize = 10;

            var users = us.Users.AsQueryable();

            // Lọc theo Họ tên
            if (!string.IsNullOrEmpty(FullName))
            {
                users = users.Where(u => u.FullName.Contains(FullName));
            }

            // Giữ lại giá trị tìm kiếm
            ViewBag.FullName = FullName;

            var pageList = users
                .OrderByDescending(u => u.UserId)
                .ToPagedList(pageNumber, pageSize);

            return View(pageList);
        }



        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.Avatar = FileName;
                }
            }
            if (!ModelState.IsValid)
                return View(data);

            // ✅ Mã hóa mật khẩu bằng BCrypt
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);

            data.CreatedAt = DateTime.Now;
            us.Users.Add(data);
            us.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var user = us.Users.Find(id);
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User data)
        {
            var files = HttpContext.Request.Form.Files;
            var FileName = "";
            //using System.Linq;
            if (files.Count() > 0 && files[0].Length > 0)
            {
                var file = files[0];
                FileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    data.Avatar = FileName;
                }
            }
            var existingUser = us.Users.FirstOrDefault(u => u.UserId == data.UserId);
            if (existingUser == null)
                return NotFound();

            // ✅ Nếu nhập mật khẩu mới → mã hóa
            if (!string.IsNullOrEmpty(data.Password))
            {
                data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            }
            else
            {
                // Giữ nguyên mật khẩu cũ
                data.Password = existingUser.Password;
            }

            // Giữ lại CreatedAt cũ
            data.CreatedAt = existingUser.CreatedAt;

            us.Entry(existingUser).CurrentValues.SetValues(data);
            us.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var usr = us.Users.Find(id);
            us.Users.Remove(usr);
            us.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allUsers = us.Users.ToList();
            us.Users.RemoveRange(allUsers);
            us.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("FullName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(User data)
        {
            if (HttpContext.Session.GetString("FullName") == null)
            {
                var user = us.Users.FirstOrDefault(u => u.FullName == data.FullName);

                if (user != null)
                {
                    // ✅ So sánh mật khẩu nhập vào với hash trong DB
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(data.Password, user.Password);

                    if (isPasswordValid)
                    {
                        HttpContext.Session.SetString("FullName", user.FullName);
                        HttpContext.Session.SetString("UserName", user.Username);
                        HttpContext.Session.SetInt32("UserId", user.UserId);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }

            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
