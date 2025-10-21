using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class LogonController : Controller
    {
        private ONLINE_GALLERY_ART_SYSTEMContext bkap = new ONLINE_GALLERY_ART_SYSTEMContext();
        private static readonly List<Admin> StaticUsers = new List<Admin>
        {
            new Admin { UserName = "admin", Password = "123456", Name = "Admin" },
        };

        public IActionResult Index(int pageNumber = 1)
        {
            var pageSize = 10;
            var pageList = bkap.Admins.OrderBy(ct => ct.Id).ToPagedList(pageNumber, pageSize);
            return View(pageList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Admin data)
        {
            // Hash mật khẩu trước khi lưu
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);

            bkap.Admins.Add(data);
            bkap.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Edit(int id)
        {
            var user = bkap.Admins.Find(id);
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(Admin data)
        {
            bkap.Admins.Update(data);
            bkap.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var user = bkap.Admins.Find(id);
            bkap.Admins.Remove(user);
            bkap.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allUsers = bkap.Admins.ToList();
            bkap.Admins.RemoveRange(allUsers);
            bkap.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logon(string username, string pass)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass))
            {
                // Nếu username hoặc pass rỗng, trả về lỗi
                ViewBag.ErrorMessage = "Tên đăng nhập và mật khẩu không được để trống.";
                return View("Index");
            }

            // 1. Kiểm tra trong danh sách tĩnh
            var staticUser = StaticUsers.FirstOrDefault(u => u.UserName == username && u.Password == pass);
            if (staticUser != null)
            {
                HttpContext.Session.SetString("username", staticUser.UserName);
                HttpContext.Session.SetString("hoten", staticUser.Name);
                return RedirectToAction("AdminIndex", "Admin");
            }

            // 2. Kiểm tra trong cơ sở dữ liệu
            Admin obj = bkap.Admins.FirstOrDefault(u => u.UserName == username);
            if (obj != null)
            {
                // So sánh mật khẩu người dùng nhập vào với mật khẩu đã mã hóa trong CSDL
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(pass, obj.Password);   // So sánh mật khẩu đã mã hóa trong CSDL với mật khẩu nhập vào

                if (isPasswordValid)
                {
                    // Mật khẩu đúng, lưu thông tin vào session và chuyển hướng
                    HttpContext.Session.SetString("username", obj.UserName);
                    HttpContext.Session.SetString("hoten", obj.Name);
                    return RedirectToAction("AdminIndex", "Admin");
                }
            }

            // 3. Nếu không tìm thấy ở cả hai nơi, trả về lỗi
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa tất cả dữ liệu trong session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Index", "Admin");
        }
    }
}
