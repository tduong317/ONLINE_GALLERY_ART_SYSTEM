using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System.Linq;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class ContactController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext _context;

        public ContactController(ONLINE_GALLERY_ART_SYSTEMContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 5;
            var contacts = _context.Contacts
                .OrderByDescending(c => c.SentAt)
                .ToPagedList(pageNumber ?? 1, pageSize);
            return View(contacts);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.SentAt = DateTime.Now;
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        public IActionResult Edit(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null) return NotFound();
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contact contact)
        {
            if (id != contact.MessageId) return NotFound();
            if (ModelState.IsValid)
            {
                contact.SentAt = DateTime.Now;
                _context.Update(contact);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        public IActionResult Delete(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            var allContact = _context.Contacts.ToList();
            _context.Contacts.RemoveRange(allContact);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}