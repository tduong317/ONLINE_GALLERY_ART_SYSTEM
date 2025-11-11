using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var banner = _context.Banners.ToList();
            ViewBag.Banners = banner;
            List<Config> cf = _context.Configs.Where(cf => cf.Id == 1).ToList();
            ViewBag.Config = cf;
            List<Exhibition> ex = _context.Exhibitions
                .OrderByDescending(e => e.StartDate)
                .Take(3)
                .ToList();
            ViewBag.Exhibitions = ex;
            var artwork = _context.Artworks
                .Include(a => a.Category) // Gọi luôn thông tin category
                .OrderBy(a => a.ArtworkId)
                .Take(6)
                .ToList();
            
            return View(artwork);
        }

        public IActionResult Exhibition(string? query, int page = 1)
        {
            int limit = 3;

            // Bắt đầu truy vấn Exhibition
            var exhibitions = _context.Exhibitions.AsQueryable();

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(query))
            {
                exhibitions = exhibitions.Where(e =>
                    e.Name.Contains(query) ||
                    (e.Location != null && e.Location.Contains(query)) ||
                    (e.Description != null && e.Description.Contains(query))
                );

                ViewBag.Query = query; // Giữ lại từ khóa để hiển thị trong ô tìm kiếm
            }

            // Thực hiện phân trang
            var pagedList = exhibitions
                .OrderByDescending(e => e.StartDate)
                .ToPagedList(page, limit);

            return View(pagedList);
        }
        public IActionResult Artwork(int page = 1, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            int limit = 6;

            // Lấy danh sách category để hiển thị ở sidebar
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            // Lưu lại giá trị filter để hiển thị lại trên giao diện
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Lấy toàn bộ artworks (chưa phân trang)
            var artworks = _context.Artworks
                .Include(a => a.Category)
                .Where(a => a.SaleType == "For Sale")
                .AsQueryable();

            // Lọc theo category
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
            }

            // Lọc theo giá (nếu có)
            if (minPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price <= maxPrice.Value);
            }

            // Phân trang
            var pagedArtworks = artworks.ToPagedList(page, limit);

            return View(pagedArtworks);
        }

        public async Task<IActionResult> DetailArtwork(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin chi tiết tác phẩm, kèm theo Category
            var artwork = await _context.Artworks
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.ArtworkId == id);

            if (artwork == null)
            {
                return NotFound();
            }

            // Lấy 3 tác phẩm liên quan (cùng Category, khác ID)
            var relatedArtworks = await _context.Artworks
                .Include(a => a.Category)
                .Where(a => a.ArtworkId != artwork.ArtworkId &&
                            a.SaleType == "For sale")
                .Take(3)
                .ToListAsync();

            // ✅ Gán dữ liệu sang View
            ViewBag.RelatedArtworks = relatedArtworks;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            return View(artwork);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new Contact());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "⚠️ Vui lòng nhập đầy đủ thông tin!";
                return View(contact);
            }
            try
            {
                contact.SentAt = DateTime.Now;
                _context.Contacts.Add(contact);
                _context.SaveChanges();

                string adminEmail = "nhung1379nvc@gmail.com"; // Gmail chính của bạn
                string fromEmail = adminEmail;                // Cũng là Gmail này
                string appPassword = "piyp biqc gpsi uxuq";   // App Password bạn tạo

                // Gửi email đến bạn
                string subjectAdmin = $"Liên hệ mới từ {contact.Name}";
                string bodyAdmin = $@"
            <h3>Bạn có liên hệ mới từ trang web Gallery:</h3>
            <p><b>Tên:</b> {contact.Name}</p>
            <p><b>Email:</b> {contact.Email}</p>
            <p><b>Chủ đề:</b> {contact.Subject}</p>
            <p><b>Nội dung:</b> {contact.Message}</p>
            <hr/>
            <p><i>Gửi lúc:</i> {contact.SentAt}</p>";

                // Gửi email cảm ơn lại cho người gửi
                string subjectUser = "Cảm ơn bạn đã liên hệ với Gallery 🎨";
                string bodyUser = $@"
            <p>Chào {contact.Name},</p>
            <p>Cảm ơn bạn đã gửi tin nhắn cho <b>Gallery</b>. Chúng tôi đã nhận được thông tin của bạn và sẽ phản hồi sớm nhất có thể.</p>
            <hr/>
            <p><i>Thân mến,</i><br>Đội ngũ Gallery</p>";

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(fromEmail, appPassword);

                    // Gửi mail cho admin
                    var mailToAdmin = new MailMessage(fromEmail, adminEmail, subjectAdmin, bodyAdmin);
                    mailToAdmin.IsBodyHtml = true;
                    smtp.Send(mailToAdmin);

                    // Gửi mail phản hồi cho người gửi
                    if (!string.IsNullOrEmpty(contact.Email))
                    {
                        var mailToUser = new MailMessage(fromEmail, contact.Email, subjectUser, bodyUser);
                        mailToUser.IsBodyHtml = true;
                        smtp.Send(mailToUser);
                    }
                }

                ViewBag.Message = "✅ Thank you! Your message has been sent successfully.";
                ModelState.Clear();
                return View(new Contact());
            }
            catch (Exception ex)
            {
                ViewBag.Message = "❌ Có lỗi xảy ra: " + ex.Message;
                return View(contact);
            }
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Auction(int? categoryId = null, int page = 1)
        {
            int limit = 6;

            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            var auctions = _context.Auctions
                .Include(a => a.Artwork)
                .ThenInclude(a => a.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                // ✅ Lọc đúng qua Category trong Artwork
                auctions = auctions.Where(a => a.Artwork.Category.CategoryId == categoryId);
            }

            var auctionList = auctions.ToList();

            foreach (var auc in auctionList)
            {
                if (DateTime.Now < auc.StartTime)
                    auc.Status = "Upcoming";
                else if (DateTime.Now >= auc.StartTime && DateTime.Now <= auc.EndTime)
                    auc.Status = "Active";
                else
                    auc.Status = "Closed";
            }

            var pagedAuctions = auctionList.ToPagedList(page, limit);

            return View(pagedAuctions);
        }


        public async Task<IActionResult> DetailAuction(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Artwork)
                .ThenInclude(art => art.Category)
                .Include(a => a.Bids)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.AuctionId == id);

            if (auction == null)
            {
                return NotFound();
            }
            var relatedAuctions = await _context.Auctions
            .Include(a => a.Artwork)
            .Where(a => a.AuctionId != auction.AuctionId &&
                        a.Artwork.CategoryId == auction.Artwork.CategoryId)
            .Take(3)
            .ToListAsync();

            // ✅ Gán vào ViewBag
            ViewBag.RelatedAuctions = relatedAuctions;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(int AuctionId, decimal BidAmount)
        {
            // 1️⃣ Lấy auction
            var auction = await _context.Auctions
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.AuctionId == AuctionId);

            if (auction == null)
            {
                TempData["BidError"] = "Auction not found.";
                return RedirectToAction("DetailAuction", new { id = AuctionId });
            }

            // 2️⃣ Lấy UserId từ session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                string returnUrl = Request.Form["returnUrl"];
                return RedirectToAction("Login", "User", new { returnUrl });
            }


            // 3️⃣ Kiểm tra giá hợp lệ
            decimal currentPrice = auction.CurrentPrice ?? auction.StartPrice;
            if (BidAmount <= currentPrice)
            {
                TempData["BidError"] = $"Your bid must be greater than {currentPrice:C}";
                return RedirectToAction("DetailAuction", new { id = AuctionId });
            }

            // 4️⃣ Tạo bid mới
            var bid = new Bid
            {
                AuctionId = AuctionId,
                UserId = userId.Value,
                Amount = BidAmount,
                BidTime = DateTime.Now
            };

            try
            {
                // 5️⃣ Cập nhật giá mới
                auction.CurrentPrice = BidAmount;

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync(); // SaveChangesAsync sẽ lưu cả Bid + CurrentPrice
                TempData["BidSuccess"] = $"Your bid of {BidAmount:C} has been placed successfully!";
            }
            catch (Exception ex)
            {
                TempData["BidError"] = "Error placing bid: " + ex.Message;
            }

            return RedirectToAction("DetailAuction", new { id = AuctionId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBid(int BidId)
        {
            // 1️⃣ Lấy UserId từ session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["BidError"] = "You must log in to cancel a bid.";
                return RedirectToAction("Login", "User", new { returnUrl = Request.Path });
            }

            // 2️⃣ Lấy bid, đảm bảo thuộc về user
            var bid = await _context.Bids
                .FirstOrDefaultAsync(b => b.BidId == BidId && b.UserId == userId);

            if (bid == null)
            {
                TempData["BidError"] = "Bid not found or you cannot cancel it.";
                return RedirectToAction("Auction"); // Hoặc redirect về list auction
            }

            // 3️⃣ Kiểm tra thời gian đấu giá
            var auction = await _context.Auctions.FindAsync(bid.AuctionId);
            if (auction.EndTime <= DateTime.Now)
            {
                TempData["BidError"] = "You cannot cancel a bid after the auction has ended.";
                return RedirectToAction("DetailAuction", new { id = auction.AuctionId });
            }

            // 4️⃣ Xoá bid
            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();

            // 5️⃣ Cập nhật lại CurrentPrice = highest bid còn lại
            var highestBid = await _context.Bids
                .Where(b => b.AuctionId == auction.AuctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();

            auction.CurrentPrice = highestBid?.Amount ?? auction.StartPrice;
            await _context.SaveChangesAsync();

            TempData["BidSuccess"] = "Your bid has been canceled successfully.";
            return RedirectToAction("DetailAuction", new { id = auction.AuctionId });
        }

        public IActionResult DetailExh(int? id)
        {
            // 1️⃣ Kiểm tra id hợp lệ
            if (id == null)
            {
                return NotFound();
            }

            // 2️⃣ Truy vấn triển lãm có kèm dữ liệu liên quan (nếu có quan hệ)
            var exhibition = _context.Exhibitions
                .Include(e => e.Reviews)
                .ThenInclude(u => u.User)
                .FirstOrDefault(e => e.ExhibitionId == id);

            // 3️⃣ Nếu không tìm thấy thì báo lỗi 404
            if (exhibition == null)
            {
                return NotFound();
            }

            // 4️⃣ Gửi dữ liệu sang View để hiển thị
            return View(exhibition);
        }

    }
}
