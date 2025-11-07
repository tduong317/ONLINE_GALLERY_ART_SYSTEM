using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Artwork(int page = 1, int? categoryId = null)
        {
            int limit = 6;

            // Lấy danh sách category để hiển thị ở sidebar
            ViewBag.CategoryList = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;
            // Lấy toàn bộ artworks (chưa phân trang)
            var artworks = _context.Artworks
               .Include(a => a.Category)
               .Where(a => a.SaleType == "For Sale") // 👈 lọc ngay từ đầu
               .AsQueryable();

            // Nếu có categoryId thì lọc theo category đó
            if (categoryId.HasValue && categoryId > 0)
            {
                artworks = artworks.Where(a => a.CategoryId == categoryId);
                ViewBag.SelectedCategoryId = categoryId; // để biết đang chọn category nào
            }

            // Cuối cùng mới phân trang
            var pagedArtworks = artworks.ToPagedList(page, limit);

            return View(pagedArtworks);
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
    }
}
