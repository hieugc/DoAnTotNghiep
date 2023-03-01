using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Hãy điền Email đăng nhập")]
        [MaxLength(320, ErrorMessage = "Email tối đa 320 ký tự")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Hãy điền mật khẩu")]
        [MaxLength(32, ErrorMessage = "Mật khẩu tối đa 32 ký tự")]
        [MinLength(8, ErrorMessage = "Mật khẩu it nhất 8 ký tự")]
        public string Password { get; set; } = string.Empty;
    }
}
