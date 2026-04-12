using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Khai báo Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. TẠM THỜI XÓA DATABASE CŨ BỊ LỆCH PHA
    //db.Database.EnsureDeleted();

    // 2. TẠO LẠI DATABASE MỚI THEO ĐÚNG CẤU TRÚC CODE HIỆN TẠI (Không cần dùng Migrate nữa)
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();