using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context = new ONLINE_GALLERY_ART_SYSTEMContext();
        public IActionResult Index(int pageNumber = 1)
        {
            var pageSize = 10;
            var pageList = _context.Categories.OrderBy(cate => cate.CategoryId).ToPagedList(pageNumber, pageSize);
            return View(pageList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category cate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cate);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = await _context.Categories.FindAsync(id);
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category cate)
        {
            cate.CreatedAt = DateTime.Now;
            if (id != cate.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(c => c.CategoryId== cate.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cate);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var cate = await _context.Categories.FindAsync(id);
            if (cate != null)
            {
                _context.Categories.Remove(cate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            var allCates = _context.Categories.ToList();
            _context.Categories.RemoveRange(allCates);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
