using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class BidController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();

        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 6;
            var bids = _context.Bids
                .Include(b => b.Auction)
                .ThenInclude(a => a.Artwork)
                .Include(b => b.User)
                .OrderByDescending(b => b.BidTime)
                .ToPagedList(pageNumber ?? 1, pageSize);

            return View(bids);
        }
    }
}
