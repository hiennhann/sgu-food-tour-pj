using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGU_FOOD_TOUR_PJ.data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- BƯỚC 1: CẤU HÌNH SWAGGER (Đã làm sạch để không bị lỗi) ---
builder.Services.AddSwaggerGen();

// --- BƯỚC 2: ĐĂNG KÝ DATABASE (POSTGRESQL) ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- BƯỚC 3: ĐĂNG KÝ JWT AUTHENTICATION ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = false, // Trong thực tế có thể bật lên
        ValidateAudience = false // Trong thực tế có thể bật lên
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// --- BƯỚC 4: KÍCH HOẠT MIDDLEWARE (THỨ TỰ CỰC KỲ QUAN TRỌNG) ---
// Bắt buộc phải là Authentication (Xác thực ai) TRƯỚC Authorization (Quyền gì)
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();