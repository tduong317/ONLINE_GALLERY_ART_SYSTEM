using Gallery_Art_System.Helper;
using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class UserController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        public IActionResult Index(string? FullName, int pageNumber = 1)
        {
            int pageSize = 10;

            var users = _context.Users.AsQueryable();

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
            _context.Users.Add(data);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
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
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == data.UserId);
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

            _context.Entry(existingUser).CurrentValues.SetValues(data);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var usr = _context.Users.Find(id);
            _context.Users.Remove(usr);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            var allUsers = _context.Users.ToList();
            _context.Users.RemoveRange(allUsers);
            _context.SaveChanges();
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

            _context.Users.Add(data);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login");
        }
        public IActionResult Login(string? returnUrl)
        {
            // Lưu đường dẫn quay lại (returnUrl) để gửi qua View
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string identifier, string password, string? returnUrl)
        {
            User user = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                user = identifier.Contains("@") ?
                       _context.Users.FirstOrDefault(u => u.Email == identifier) :
                       _context.Users.FirstOrDefault(u => u.Username == identifier);
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid username/email or password!";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        [HttpGet]
        public IActionResult Logout(string? returnUrl)
        {
            // ✅ Xóa toàn bộ session đăng nhập
            HttpContext.Session.Clear();

            // ✅ Nếu có returnUrl và là URL nội bộ → quay lại đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // ✅ Ngược lại, chuyển về trang Login
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                // Nếu bạn dùng Session để lấy user hiện tại:
                id = HttpContext.Session.GetInt32("UserId");
                if (id == null) return RedirectToAction("Login", "User");
            }

            var user = await _context.Users
                .Include(u => u.Artworks)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // =======================
        // Edit - hiển thị form chỉnh sửa
        // =======================
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null)
            {
                id = HttpContext.Session.GetInt32("UserId");
                if (id == null) return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // =======================
        // Edit (POST) - cập nhật profile
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("UserId,Username,FullName,Gender,Age,Email,Phone,Address,Avatar")] User updatedUser)
        {
            if (id != updatedUser.UserId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null) return NotFound();

                    // Cập nhật từng trường
                    user.Username = updatedUser.Username;
                    user.FullName = updatedUser.FullName;
                    user.Gender = updatedUser.Gender;
                    user.Age = updatedUser.Age;
                    user.Email = updatedUser.Email;
                    user.Phone = updatedUser.Phone;
                    user.Address = updatedUser.Address;
                    user.Avatar = updatedUser.Avatar;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = user.UserId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Cannot update profile now.");
                }
            }

            return View(updatedUser);
        }

        // =======================
        // Optional: Xem review của user
        // =======================
        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                id = HttpContext.Session.GetInt32("UserId");
                if (id == null) return RedirectToAction("Login", "User");
            }

            var reviews = await _context.Reviews
                .Where(r => r.UserId == id)
                .Include(r => r.Artwork)
                .Include(r => r.Exhibition)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.UserId = id;
            return View(reviews);
        }
    }
}
