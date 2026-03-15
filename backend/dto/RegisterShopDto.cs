namespace SGU_FOOD_TOUR_PJ.dto
{
    public class RegisterShopDto
    {
        // Thông tin tài khoản
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Thông tin quán ăn
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
    }
}