using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền email")]
        [MaxLength(320, ErrorMessage = "Email tối đa 320 ký tự")]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền mật khẩu")]
        [MaxLength(100)]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy mã hóa mật khẩu")]
        [MaxLength(100)]
        [Column("salt")]
        public string Salt { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền họ")]
        [MaxLength(30, ErrorMessage = "Họ tối đa 30 ký tự")]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền tên")]
        [MaxLength(30, ErrorMessage = "Tên tối đa 30 ký tự")]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền số điện thoại")]
        [MaxLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [MinLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("bonus_point")]
        public int BonusPoint { get; set; } = 0;
        [Column("point")]
        public int Point { get; set; } = 0;

        [Column("id_file")]
        public virtual int? IdFile { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }
    }
}
