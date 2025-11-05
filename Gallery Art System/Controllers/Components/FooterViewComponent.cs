using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gallery_Art_System.Controllers.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext bkap = new ONLINE_GALLERY_ART_SYSTEMContext();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var page1 = bkap.Pages
                .Where(p => p.Position == 2 && p.Status == true)
                .OrderBy(p => p.Ord)
                .ToList();
            var parents1 = page1.Where(p => p.Level.Length == 5).ToList();
            var children1 = page1.Where(p => p.Level.Length > 5).ToList();
            var menu1 = parents1.Select(p => new
            {
                Parent = p,
                Children = children1.Where(c => c.Level.StartsWith(p.Level)).ToList()
            }).ToList();
            ViewBag.Page1 = menu1;

            var page2 = bkap.Pages
                .Where(p => p.Position == 3 && p.Status == true)
                .OrderBy(p => p.Ord)
                .ToList();
            var parents2 = page2.Where(p => p.Level.Length == 5).ToList();
            var children2 = page2.Where(p => p.Level.Length > 5).ToList();
            var menu2 = parents2.Select(p => new
            {
                Parent = p,
                Children = children2.Where(c => c.Level.StartsWith(p.Level)).ToList()
            }).ToList();
            ViewBag.Page2 = menu2;
            List<Config> cf = bkap.Configs.Where(cf => cf.Id == 1).ToList();
            ViewBag.Config = cf;
            return View();
        }
    }
}
