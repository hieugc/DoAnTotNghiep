using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền email")]
        [MaxLength(320, ErrorMessage = "Email tối đa 320 ký tự")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền mật khẩu")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy mã hóa mật khẩu")]
        [MaxLength(100)]
        public string Salt { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền họ")]
        [MaxLength(30, ErrorMessage = "Họ tối đa 30 ký tự")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền tên")]
        [MaxLength(30, ErrorMessage = "Tên tối đa 30 ký tự")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền số điện thoại")]
        [MaxLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [MinLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        public string PhoneNumber { get; set; } = string.Empty;

        public int BonusPoint { get; set; } = 0;
        public int Point { get; set; } = 0;
    }
}
