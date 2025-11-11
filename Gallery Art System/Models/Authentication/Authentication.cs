using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Gallery_Art_System.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Nếu chưa đăng nhập
            if (session.GetString("FullName") == null)
            {
                // Lấy đường dẫn hiện tại để quay lại
                var currentController = context.RouteData.Values["controller"]?.ToString();
                var currentAction = context.RouteData.Values["action"]?.ToString();

                // Tạo returnUrl kiểu "/Controller/Action"
                var returnUrl = "/" + currentController + "/" + currentAction;

                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "User" },
                        { "Action", "Login" },
                        { "returnUrl", returnUrl } // gắn returnUrl vào query string
                    });
            }
        }
    }
}
