using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery_Art_System.Controllers.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext bkap = new ONLINE_GALLERY_ART_SYSTEMContext();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            List<Config> cf = bkap.Configs.Where(cf => cf.Id == 1).ToList();
            ViewBag.Config = cf;
            return View();
        }
    }
}
