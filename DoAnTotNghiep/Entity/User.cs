using DoAnTotNghiep.Data;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("User")]
    public class User
    {
        public void UpdateInfoUser(UpdateUserInfo model)
        {
            this.BirthDay = model.BirthDay;
            this.LastName = model.LastName;
            this.FirstName= model.FirstName;
            this.Gender = model.Gender;
            this.PhoneNumber = model.PhoneNumber;
            this.Email = model.Email;
        }
        public void UpdateInfoUser(RegisterInfoViewModel model)
        {
            this.BirthDay = model.BirthDay;
            this.LastName = model.LastName;
            this.FirstName = model.FirstName;
            this.Gender = model.Gender;
            this.PhoneNumber = model.PhoneNumber;
        }
        public void InCludeAll(DoAnTotNghiepContext context)
        {
            if (this.Houses == null && !context.Entry(this).Collection(m => m.Houses).IsLoaded) context.Entry(this).Collection(m => m.Houses).Load();
            if (this.Files == null && !context.Entry(this).Reference(m => m.Files).IsLoaded) context.Entry(this).Reference(m => m.Files).Load();
        }
        public static User CreateUserByRegister(string email, string password, string salt)
        {
            return new User
            {
                Email = email,
                Password = password,
                Salt = salt,
                PhoneNumber = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Point = 0,
                BonusPoint = 0,
                IdFile = null,
                BirthDay = DateTime.UtcNow,
                Gender = false
            };
        }

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

        [MaxLength(30, ErrorMessage = "Họ tối đa 30 ký tự")]
        [Column("last_name")]
        public string? LastName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Tên tối đa 50 ký tự")]
        [Column("first_name")]
        public string? FirstName { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [MinLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Column("bonus_point")]
        public int BonusPoint { get; set; } = 0;
        [Column("point")]
        public int Point { get; set; } = 0;

        [Column("role")]
        public int Role { get; set; } = 0;

        [Column("gender")]
        public bool? Gender { get; set; } = false;
        [Column("birth_day")]
        public DateTime? BirthDay { get; set; } = DateTime.Now;

        [Column("user_rating")]
        public double UserRating { get; set; } = 0;

        [Column("point_using")]
        public int PointUsing { get; set; } = 0;

        [Column("number_user_rating")]
        public int NumberUserRating { get; set; } = 0;

        [Column("id_file")]
        public virtual int? IdFile { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }

        public virtual ICollection<UsersInChatRoom>? ChatRoom { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<House>? Houses { get; set; }
    }
}
