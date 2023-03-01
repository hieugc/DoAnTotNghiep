using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Hãy điền email")]
        [MaxLength(320, ErrorMessage = "Email tối đa 320 ký tự")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền mật khẩu")]
        [MaxLength(32, ErrorMessage = "Mật khẩu tối đa 32 ký tự")]
        [MinLength(8, ErrorMessage = "Mật khẩu ít nhất 8 ký tự")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Hãy xác nhận mật khẩu")]
        [MaxLength(32, ErrorMessage = "Mật khẩu tối đa 32 ký tự")]
        [MinLength(8, ErrorMessage = "Mật khẩu ít nhất 8 ký tự")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền họ và tên đệm")]
        [MaxLength(30, ErrorMessage = "Họ và tên đệm tối đa 30 ký tự")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền tên của bạn")]
        [MaxLength(30, ErrorMessage = "Tên tối đa 30 ký tự")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền số điện thoại")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Số điện thoại 10 số")]
        [MaxLength(10, ErrorMessage = "Số điện thoại 10 số")]
        [MinLength(10, ErrorMessage = "Số điện thoại 10 số")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
