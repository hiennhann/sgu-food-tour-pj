namespace VinhKhanhTour.Models
{
    public class FoodPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // DÒNG NÀY LÀ THỦ PHẠM GÂY LỖI: Thêm nó vào để chứa Danh mục
        public string Category { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string ImageUrl { get; set; }
        public string NarrationText { get; set; }
    }
}