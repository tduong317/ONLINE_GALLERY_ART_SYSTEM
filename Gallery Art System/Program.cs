using Gallery_Art_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ===== Add services =====

// Cho phép dùng IHttpContextAccessor trong View và Controller
builder.Services.AddHttpContextAccessor();

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Session hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Thêm Controllers + Views
builder.Services.AddControllersWithViews();

// Kết nối Database
builder.Services.AddDbContext<ONLINE_GALLERY_ART_SYSTEMContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình dung lượng upload tối đa
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 500L * 1024 * 1024 * 1024; // 500GB
});

// ===== Add Authentication (Cookie) =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // đường dẫn trang login
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

var app = builder.Build();

// ===== Configure middleware =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Bật Session (phải đặt sau UseRouting và trước MapControllerRoute)
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
