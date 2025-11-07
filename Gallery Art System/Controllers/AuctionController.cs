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
            foreach (var auc in auctions)
            {
                if (DateTime.Now < auc.StartTime)
                    auc.Status = "Upcoming";
                else if (DateTime.Now >= auc.StartTime && DateTime.Now <= auc.EndTime)
                    auc.Status = "Active";
                else
                    auc.Status = "Closed";
            }
            _context.SaveChanges();

            return View(auctions);
        }

        public IActionResult GetArtworkPrice(int id)
        {
            var artwork = _context.Artworks
                .Where(a => a.ArtworkId == id)
                .Select(a => new { a.Price })
                .FirstOrDefault();

            if (artwork == null)
                return NotFound();

            return Json(new { price = artwork.Price });
        }
        public IActionResult Create()
        {
            ViewData["ArtworkId"] = new SelectList(
                _context.Artworks
                    .Where(a =>
                        a.SaleType == "Auction" &&                         // chỉ lấy artwork kiểu đấu giá
                        !_context.Auctions.Any(auc => auc.ArtworkId == a.ArtworkId) // chưa được thêm vào auction
                    ),
                "ArtworkId",
                "Title"
            );
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Auction auction)
        {
            // Loại bỏ validation của Artwork navigation property
            ModelState.Remove("Artwork");

            // Nếu Status chưa được gán thì mặc định là false (chưa Active)
            auction.Status = "Upcoming";

            // Gán UserId = null, vì lúc tạo có thể chưa có người tham gia
            auction.UserId = null;

            if (ModelState.IsValid)
            {
                // Kiểm tra xem Artwork đã có đấu giá chưa (và chưa bị Reject)
                bool isArtworkAuctioned = _context.Auctions.Any(a =>
                    a.ArtworkId == auction.ArtworkId &&
                    a.ApprovalStatus != "Rejected"
                );

                if (isArtworkAuctioned)
                {
                    ModelState.AddModelError("ArtworkId",
                        "❌ This artwork has already been auctioned and cannot be auctioned again.");
                }
                else
                {
                    // Khi tạo mới, đấu giá chờ duyệt
                    auction.ApprovalStatus = "Pending";

                    // Status = false → chưa hoạt động, EndTime / StartTime sẽ quyết định Active sau

                    auction.Status = "Upcoming";

                    // Lấy artwork tương ứng để cập nhật giá
                    var artwork = await _context.Artworks.FindAsync(auction.ArtworkId);
                    if (artwork != null)
                    {
                        artwork.Price = auction.StartPrice; // cập nhật giá Artwork theo StartPrice của Auction
                        _context.Artworks.Update(artwork);
                    }

                    // Lưu vào DB
                    _context.Add(auction);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Your auction has been submitted and is awaiting admin approval.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Load lại dropdown các Artwork chưa được đấu giá
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
                    var artwork = await _context.Artworks.FindAsync(auction.ArtworkId);
                    if (artwork != null)
                    {
                        artwork.Price = auction.StartPrice;
                        _context.Artworks.Update(artwork);
                    }

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
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

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
            auction.Status = "Active"; // kích hoạt đấu giá
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
            auction.Status = "Closed"; // bị từ chối
            await _context.SaveChangesAsync();

            TempData["Message"] = "❌ The auction has been rejected.";
            return RedirectToAction(nameof(Index));
        }

    }
}
