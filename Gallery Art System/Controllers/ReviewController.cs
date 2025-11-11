using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 5;
            var reviews = await _context.Reviews
                .Include(r => r.Artwork)       // nếu muốn hiển thị ảnh artwork
                .Include(r => r.Exhibition)    // nếu muốn hiển thị ảnh exhibition
                .ToListAsync();
            var pageList = _context.Reviews.ToPagedList(page,limit);
            return View(pageList);
        }


        [HttpPost]
        public async Task<IActionResult> AddReview(int exhibitionId, int rating, string? comment)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Chưa login → redirect sang login, kèm returnUrl về page hiện tại
                return RedirectToAction("Login", "User", new
                {
                    returnUrl = Url.Action("DetailExh", "Home", new { id = exhibitionId })
                });
            }

            // Lấy user từ DB
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var review = new Review
            {
                ExhibitionId = exhibitionId,
                UserId = userId.Value,
                Name = user.FullName ?? user.Username,
                Email = user.Email,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailExh", "Home", new { id = exhibitionId });
        }


        // =======================
        // Delete
        // =======================
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // =======================
        // DeleteAll 
        // =======================
        public async Task<IActionResult> DeleteAll()
        {
            var allReviews = await _context.Reviews.ToListAsync();
            _context.Reviews.RemoveRange(allReviews);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
