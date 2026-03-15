using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGU_FOOD_TOUR_PJ.controllers
{
    [Authorize(Roles = "SHOP_OWNER")]
    [ApiController]
    [Route("api/owner/upload")]
    public class OwnerUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        // Chỉ cần IWebHostEnvironment để lưu file, không cần gọi Database ở đây
        public OwnerUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn một file hợp lệ.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Chỉ chấp nhận định dạng ảnh (jpg, jpeg, png, webp).");

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/{uniqueFileName}";
            
            return Ok(new { message = "Upload thành công!", url = fileUrl });
        }
    }
}