using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();

        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 6;
            var auctions = _context.Auctions
                .Include(a => a.Artwork)   // ✅ Load Artwork để tránh null
                .OrderByDescending(a => a.AuctionId)
                .ToPagedList(pageNumber ?? 1, pageSize);

            return View(auctions);
        }
        public IActionResult Create()
        {

            ViewData["ArtworkId"] = new SelectList(
                _context.Artworks
                .Where(a => !_context.Auctions.Any(auc => auc.ArtworkId == a.ArtworkId)),
                "ArtworkId",
                "Title"
            );
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Auction auction)
        {
            ModelState.Remove("Artwork");
            if (auction.Status == null)
            {
                auction.Status = false; 
            }
            if (ModelState.IsValid)
            {
                // ✅ Kiểm tra xem Artwork đã có trong Auction chưa (và chưa bị Reject)
                bool isArtworkAuctioned = _context.Auctions.Any(a =>
                    a.ArtworkId == auction.ArtworkId &&
                    a.ApprovalStatus != "Rejected" // vẫn có thể tạo lại nếu bị reject
                );

                if (isArtworkAuctioned)
                {
                    // Báo lỗi hiển thị trên View
                    ModelState.AddModelError("ArtworkId", "❌ This artwork has already been auctioned and cannot be auctioned again.");
                }
                else
                {
                    // ✅ Thiết lập trạng thái ban đầu khi user tạo auction
                    auction.StartTime = DateTime.Now;
                    auction.Status = false; // chưa hoạt động
                    auction.ApprovalStatus = "Pending"; // chờ duyệt bởi admin

                    _context.Add(auction);
                    await _context.SaveChangesAsync();

                    // ✅ Gửi thông báo cho người dùng
                    TempData["Message"] = "Your auction has been submitted and is awaiting admin approval.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // ✅ Nếu có lỗi thì load lại dropdown Artwork
            ViewData["ArtworkId"] = new SelectList(
                _context.Artworks
                .Where(a => !_context.Auctions.Any(auc =>
                    auc.ArtworkId == a.ArtworkId &&
                    auc.ApprovalStatus != "Rejected"
                )),
                "ArtworkId",
                "Title",
                auction.ArtworkId
            );

            return View(auction);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            ViewData["ArtworkId"] = new SelectList(_context.Artworks, "ArtworkId", "Title", auction.ArtworkId);
            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Auction auction)
        {
            ModelState.Remove("Artwork");
            if (id != auction.AuctionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // ✅ Lấy bản ghi cũ từ DB
                    var existingAuction = await _context.Auctions.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AuctionId == id);
                    if (existingAuction == null)
                        return NotFound();

                    // ✅ Giữ nguyên ApprovalStatus cũ
                    auction.ApprovalStatus = existingAuction.ApprovalStatus;

                    // ✅ Cập nhật
                    _context.Update(auction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error while updating auction: " + ex.Message);
                }
            }

            ViewData["ArtworkId"] = new SelectList(_context.Artworks, "ArtworkId", "Title", auction.ArtworkId);
            return View(auction);
        }


        // POST: Auction/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Auction/DeleteAll
        [HttpPost]
        public async Task<IActionResult> DeleteAll(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return RedirectToAction(nameof(Index));

            var auctions = _context.Auctions.Where(a => ids.Contains(a.AuctionId));
            _context.Auctions.RemoveRange(auctions);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Artwork)
                .Include(a => a.Bids)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.AuctionId == id);

            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }
        // POST: Admin approves auction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            auction.ApprovalStatus = "Approved";
            auction.Status = true; // kích hoạt đấu giá
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ The auction has been approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin rejects auction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            auction.ApprovalStatus = "Rejected";
            auction.Status = false; // đảm bảo không hoạt động
            await _context.SaveChangesAsync();

            TempData["Message"] = "❌ The auction has been rejected.";
            return RedirectToAction(nameof(Index));
        }

    }
}
