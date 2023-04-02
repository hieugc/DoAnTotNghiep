using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class UpdateUserInfo{
        public UpdateUserInfo() { }
        public UpdateUserInfo(User user, string host) {
            this.LastName = user.LastName;
            this.FirstName = user.FirstName;
            this.PhoneNumber = user.PhoneNumber;
            this.BirthDay = user.BirthDay;
            this.Gender = user.Gender;
            this.Email = user.Email;
            if(user.Files != null)
            {
                this.Image = new ImageBase(user.Files, host);
            }
        }

        [Required(ErrorMessage = "Hãy điền họ và tên đệm")]
        [MaxLength(30, ErrorMessage = "Họ và tên đệm tối đa 30 ký tự")]
        public string? LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền tên của bạn")]
        [MaxLength(30, ErrorMessage = "Tên tối đa 30 ký tự")]
        public string? FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền số điện thoại")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Số điện thoại 10 số")]
        [MaxLength(10, ErrorMessage = "Số điện thoại 10 số")]
        [MinLength(10, ErrorMessage = "Số điện thoại 10 số")]
        [Phone(ErrorMessage = "Số điện thoại 10 số")]
        public string? PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Hãy điền email")]
        [MaxLength(320, ErrorMessage = "Email tối đa 320 ký tự")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy chọn ngày tháng năm sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; } = DateTime.Now;
        public bool? Gender { get; set; } = false;
        public ImageBase? Image { get; set; }
        public IFormFile? File { get; set; }
    }
}
