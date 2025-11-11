using Gallery_Art_System.Models;
using Gallery_Art_System.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gallery_Art_System.Controllers
{
    public class CartController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        private const string CART_KEY = "CART_SESSION";

        public CartController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }

        // ✅ Lấy danh sách đơn hàng trong Session (giỏ hàng)
        private List<Order> GetCart()
        {
            var json = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(json))
                return new List<Order>();

            return JsonConvert.DeserializeObject<List<Order>>(json);
        }

        // ✅ Lưu giỏ hàng vào Session
        private void SaveCart(List<Order> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, json);
        }

        // ✅ Thêm sản phẩm vào giỏ
        [HttpPost]
        public IActionResult AddToCart(int artworkId)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.ArtworkId == artworkId);
            if (artwork == null)
                return NotFound();

            var cart = GetCart();

            // Nếu sản phẩm đã có trong giỏ, chỉ cần tăng số lượng
            var existing = cart.FirstOrDefault(c => c.ArtworkId == artworkId);
            if (existing != null)
            {
                existing.TotalAmount += artwork.Price ?? 0;
            }
            else
            {
                var order = new Order
                {
                    ArtworkId = artwork.ArtworkId,
                    TotalAmount = artwork.Price ?? 0,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    Artwork = artwork
                };
                cart.Add(order);
            }

            SaveCart(cart);

            // Trả về số lượng trong giỏ (để update số hiển thị)
            return Json(new { count = cart.Count });
        }
        // ✅ Cập nhật lại số lượng trong giỏ hàng
        [HttpPost]
        public IActionResult UpdateCart([FromForm] Dictionary<int, int> quantities)
        {
            var cart = GetCart();

            foreach (var item in cart.ToList())
            {
                if (quantities.TryGetValue(item.ArtworkId, out int newQty))
                {
                    if (newQty <= 0)
                    {
                        // Nếu số lượng <= 0 thì xóa luôn
                        cart.Remove(item);
                    }
                    else
                    {
                        var artwork = _context.Artworks.FirstOrDefault(a => a.ArtworkId == item.ArtworkId);
                        if (artwork != null)
                        {
                            // Tính lại tổng tiền cho sản phẩm
                            item.TotalAmount = (artwork.Price ?? 0) * newQty;
                            item.Artwork = artwork;
                        }
                    }
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }


        // ✅ Xóa một sản phẩm khỏi giỏ
        [HttpPost]
        public IActionResult RemoveItem(int artworkId)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(c => c.ArtworkId == artworkId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }


            return RedirectToAction("Index");
        }

        // ✅ Lấy số lượng sản phẩm trong giỏ (dùng để hiển thị icon cart)
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Count });
        }
        // ✅ Trang giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    ViewBag.Address = user.Address;
                }
            }
            return View(cart);
        }
        [Authentication]  // đảm bảo phải đăng nhập
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // ✅ Lấy thông tin người dùng
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                ViewBag.Username = user.Username;
                ViewBag.FullName = user.FullName;
                ViewBag.Address = user.Address;
                ViewBag.Phone = user.Phone;
                ViewBag.Email = user.Email;
            }

            // Không lưu đơn hàng ở đây, chỉ hiển thị form xác nhận
            return View(cart);
        }
        [HttpPost]
        [Authentication]
        public IActionResult PlaceOrder([FromForm] string PaymentMethod, [FromForm] string order_notes)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index");

            // Map PaymentMethod từ form sang giá trị hợp lệ trong DB
            var methodMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "bacs", "NET_BANKING" },   // Chuyển khoản ngân hàng
        { "cod", "DEBIT_CARD" }      // COD
    };

            string dbPaymentMethod = methodMap.ContainsKey(PaymentMethod) ? methodMap[PaymentMethod] : "NET_BANKING";

            foreach (var item in cart)
            {
                // Tạo Order cho mỗi sản phẩm
                var order = new Order
                {
                    UserId = userId.Value,
                    ArtworkId = item.ArtworkId,
                    OrderDate = DateTime.Now,
                    Status = dbPaymentMethod == "DEBIT_CARD" ? "PENDING" : "PAID",
                    TotalAmount = item.TotalAmount,
                    Note = order_notes,
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                // Tạo Payment cho Order
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    Method = dbPaymentMethod,
                    PaymentDate = DateTime.Now,
                    Amount = item.TotalAmount,
                    Status = dbPaymentMethod == "DEBIT_CARD" ? "PENDING" : "SUCCESS"
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();
            }

            // Xóa giỏ hàng
            HttpContext.Session.Remove("CART_SESSION");

            TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công!";
            return RedirectToAction("OrderSuccess");
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
