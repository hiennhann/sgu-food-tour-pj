namespace VinhKhanhTour.Models
{
    public class FoodPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }             // Tên quán (đã dịch)
        public string Description { get; set; }      // Mô tả (đã dịch)
        public string Address { get; set; }          // Địa chỉ
        public double Rating { get; set; }           // Đánh giá sao
        public int ReviewCount { get; set; }         // Số lượt đánh giá
        public string ImageUrl { get; set; }         // Ảnh quán
        public string NarrationText { get; set; }    // Kịch bản TTS để máy đọc
    }
}