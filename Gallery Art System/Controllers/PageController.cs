using Gallery_Art_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using X.PagedList.Extensions;

namespace Gallery_Art_System.Controllers
{
    public class PageController : Controller
    {
        private readonly ONLINE_GALLERY_ART_SYSTEMContext bkap = new ONLINE_GALLERY_ART_SYSTEMContext();
        static string Level = "";

        // GET: Page
        // Lấy danh sách các trang với phân trang
        public ActionResult Index(int page = 1)
        {
            int pagesize = 15;  // Số lượng trang hiển thị trên mỗi trang
            var pageList = bkap.Pages
                .OrderBy(x => x.Level)   // sắp xếp theo chuỗi Level để cha trước, con sau
                .ThenBy(x => x.Ord)      // trong cùng cấp thì theo thứ tự Ord
                .ToPagedList(page, pagesize);
            return View(pageList);  // Trả về view với danh sách các trang đã phân trang
        }

        // GET: Page/Create
        // Hiển thị form tạo mới trang
        public ActionResult Create(string? strLevel)
        {
            if (strLevel != null)
                Level = strLevel;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Page data)
        {
            if (data.Status == null)
            {
                data.Status = false;
            }
            data.Tag = NameToTag(data.Name);
            if (data.Name != null)
            {


                data.Content = "";
                data.Type = 0;
                data.Index = 0;
                data.Level = Level + data.Level;
                data.Level = Level + "00000";
                Level = "";


                bkap.Pages.Add(data);
                bkap.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                List<Page> page = bkap.Pages.ToList();
                ViewBag.Pages = page;
                return View(data);
            }
        }

        // GET: Page/Edit/5
        // Hiển thị form chỉnh sửa thông tin trang theo id
        public ActionResult Edit(int id)
        {
            Page page = bkap.Pages.First(p => p.Id == id);
            if (page == null)
            {
                return NotFound();
            }
            Level = page.Level.Substring(0, page.Level.Length - 5);
            return View(page);
        }

        // POST: Page/Edit/5
        // Xử lý khi người dùng gửi form chỉnh sửa thông tin trang
        [HttpPost]
        public ActionResult Edit(Page data)
        {

            if (data.Status == null)
            {
                data.Status = false;
            }

            data.Tag = NameToTag(data.Name);
            data.Content = "";
            data.Type = 0;
            data.Index = 0;

            data.Level = Level + data.Level;
            data.Level = Level + "00000";
            Level = "";


            bkap.Pages.Update(data);
            bkap.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Page/Delete/5
        // Xử lý xóa trang theo id
        public ActionResult Delete(int id)
        {
            Page page = bkap.Pages.First(p => p.Id == id);  // Tìm trang theo id
            if (page != null)
            {
                bkap.Pages.Remove(page);  // Xóa trang khỏi cơ sở dữ liệu
                bkap.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            return RedirectToAction("Index");  // Chuyển hướng về trang danh sách trang
        }
        public IActionResult DeleteAll()
        {
            var allPages = bkap.Pages.ToList();
            bkap.Pages.RemoveRange(allPages);
            bkap.SaveChanges();
            return RedirectToAction("Index");
        }
        #region Name To Tag
        public static string NameToTag(string strName)
        {
            string strReturn = strName.Trim().ToLower();
            //strReturn = GetContent(strReturn, 150);
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            strReturn = Regex.Replace(strReturn, "[^\\w\\s]", string.Empty);
            string strFormD = strReturn.Normalize(System.Text.NormalizationForm.FormD);
            strReturn = regex.Replace(strFormD, string.Empty).Replace("đ", "d");
            strReturn = Regex.Replace(strReturn, "(-+)", " ");
            strReturn = Regex.Replace(strReturn.Trim(), "( +)", "-");
            strReturn = Regex.Replace(strReturn.Trim(), "(?+)", "");
            return strReturn;
        }
        #endregion
    }
}
