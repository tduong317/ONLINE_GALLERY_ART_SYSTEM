using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gallery_Art_System.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class ExhibitionRequestController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;

        public ExhibitionRequestController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }

        // GET: ExhibitionRequest
        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 10;
            int pageIndex = pageNumber ?? 1;

            var requests = _context.ExhibitionRequests
                .Include(r => r.User)
                .Include(r => r.Artwork)
                .Include(r => r.Exhibition)
                .OrderByDescending(r => r.RequestId)
                .ToPagedList(pageIndex, pageSize);

            return View(requests);
        }

        // GET: ExhibitionRequest/Create
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Artworks = _context.Artworks.ToList();
            ViewBag.Exhibitions = _context.Exhibitions.ToList();

            return View(new ExhibitionRequest()); // model mới tránh null
        }



        // POST: ExhibitionRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExhibitionRequest request)
        {
            // Bỏ validation của navigation properties
            ModelState.Remove("User");
            ModelState.Remove("Artwork");
            ModelState.Remove("Exhibition");

            if (ModelState.IsValid)
            {
                _context.ExhibitionRequests.Add(request);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Reload ViewBag nếu ModelState invalid
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Artworks = _context.Artworks.ToList();
            ViewBag.Exhibitions = _context.Exhibitions.ToList();

            return View(request);
        }



        // GET: ExhibitionRequest/Edit/5
        public IActionResult Edit(int id)
        {
            var request = _context.ExhibitionRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }

            ViewBag.Users = _context.Users.ToList();
            ViewBag.Artworks = _context.Artworks.ToList();
            ViewBag.Exhibitions = _context.Exhibitions.ToList();

            return View(request);
        }

        // POST: ExhibitionRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ExhibitionRequest request)
        {
            ModelState.Remove("User");
            ModelState.Remove("Artwork");
            ModelState.Remove("Exhibition");
            if (id != request.RequestId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(request);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _context.Users.ToList();
            ViewBag.Artworks = _context.Artworks.ToList();
            ViewBag.Exhibitions = _context.Exhibitions.ToList();
            return View(request);
        }

        // GET: ExhibitionRequest/Delete/5
        public IActionResult Delete(int id)
        {
            var request = _context.ExhibitionRequests.Find(id);
            if (request != null)
            {
                _context.ExhibitionRequests.Remove(request);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}