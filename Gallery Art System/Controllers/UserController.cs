using Gallery_Art_System.Helper;
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
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images/users", FileName);
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
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images//users", FileName);
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
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User data)
        {
            data.CreatedAt = DateTime.Now;

            // ✅ Dùng BCrypt trực tiếp thay vì PasswordHelper
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);

            us.Users.Add(data);
            us.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login");
        }
        public IActionResult Login(string? returnUrl)
        {
            // Nếu đã đăng nhập thì quay về Home
            if (HttpContext.Session.GetString("FullName") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lưu đường dẫn quay lại (returnUrl) để gửi qua View
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(User data, string? returnUrl)
        {
            if (HttpContext.Session.GetString("FullName") == null)
            {
                User user = null;

                // ✅ Kiểm tra theo Username trước, nếu không có thì theo Email
                if (!string.IsNullOrEmpty(data.Username))
                {
                    user = us.Users.FirstOrDefault(u => u.Username == data.Username);
                }
                else if (!string.IsNullOrEmpty(data.Email))
                {
                    user = us.Users.FirstOrDefault(u => u.Email == data.Email);
                }

                if (user != null && BCrypt.Net.BCrypt.Verify(data.Password, user.Password))
                {
                    // ✅ Lưu session sau khi đăng nhập thành công
                    HttpContext.Session.SetString("FullName", user.FullName);
                    HttpContext.Session.SetString("UserName", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.UserId);

                    // ✅ Quay lại trang trước nếu có returnUrl, ngược lại về Home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Email, username, or password is incorrect!";
                }
            }

            return View();
        }


        [HttpGet]
        public IActionResult Logout(string? returnUrl)
        {
            HttpContext.Session.Clear();

            // ✅ Nếu có returnUrl và là URL nội bộ thì quay lại trang đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // ✅ Nếu không thì về Home
            return RedirectToAction("Index", "Home");
        }

    }
}
