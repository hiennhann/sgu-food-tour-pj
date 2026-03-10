using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH BẢO MẬT JWT ---
// Khóa bí mật (phải đủ dài và phức tạp)
var securityKey = "SguFoodTourSecretKey_SieuBaoMat_2026!!!"; 
var issuer = "SguFoodTourBackend";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Tạm tắt kiểm tra người nhận
            ValidateLifetime = true,  // Kiểm tra thời hạn token
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 2. KÍCH HOẠT MIDDLEWARE BẢO MẬT ---
app.UseAuthentication(); 
app.UseAuthorization();  


// --- 3. API ĐĂNG NHẬP (TẠO TOKEN) ---
app.MapPost("/api/login", (UserLogin userInfo) =>
{
    // Tạm thời fix cứng tài khoản, sau này mình sẽ thay bằng Database
    if (userInfo.Username == "admin" && userInfo.Password == "123456")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), // Token sống được 2 tiếng
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtString = tokenHandler.WriteToken(token);

        return Results.Ok(new { message = "Đăng nhập thành công!", token = jwtString });
    }
    
    return Results.Unauthorized(); // Trả về lỗi 401 nếu sai tài khoản
});


// --- 4. API BẢO MẬT (YÊU CẦU TOKEN) ---
// Thêm [Authorize] để khóa API này lại
app.MapGet("/api/profile", [Authorize] () =>
{
    return Results.Ok(new { 
        message = "Chào mừng bạn đến với hệ thống quản lý SGU Food Tour!",
        status = "Thành công"
    });
});

app.Run();

// --- 5. ĐỊNH NGHĨA DỮ LIỆU ĐẦU VÀO ---
class UserLogin
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}