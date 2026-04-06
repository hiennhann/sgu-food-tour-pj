using SQLite;

namespace VinhKhanhTour.Models
{
    // Bảng này sẽ lưu trữ thông tin của khách hàng
    [Table("UserAccount")]
    public class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        // Unique: Đảm bảo không có 2 người dùng chung 1 email
        [Unique, MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }
    }
}