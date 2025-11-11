using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery_Art_System.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;

        public ProfileController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }

        // =======================
        // Index - xem profile
        // =======================
        public async Task<IActionResult> Index(int? id)
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
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,FullName,Gender,Age,Email,Phone,Address,Avatar")] User updatedUser)
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
