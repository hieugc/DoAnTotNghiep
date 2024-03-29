﻿using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;

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

    public class ApiLoginData
    {
        public string token { get; set; } = string.Empty;
        public DateTime expire { get; set; } = DateTime.Now;
        public UserInfo? userInfo { get; set; }
    }

    public class ApiBoolean
    {
        public ApiBoolean(bool value)
        {
            this.isExisted = value;
        }
        public bool isExisted { get; set; } = false;
    }

    public class UserInfo
    {
        public UserInfo() { }
        public UserInfo(User user, byte[] salt, string host)
        {
            this.PhoneNumber = user.PhoneNumber;
            this.Email = user.Email;
            this.BirthDay = user.BirthDay;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Gender = user.Gender;
            this.UserAccess = Crypto.EncodeKey(user.Id.ToString(), salt);
            this.UrlImage = user.Files == null ? null : (host + "/" + user.Files.PathFolder + "/" + user.Files.FileName);
            this.Point = user.BonusPoint + user.Point;
            this.UserRating = user.UserRating;
            this.NumberRating = user.NumberUserRating;
            if (user.Houses != null)
            {
                this.NumberOfHouses = user.Houses.Count;
            }
        }

        public static void GetEntityRelated(User user, DoAnTotNghiepContext context)
        {
            if(!context.Entry(user).Collection(m => m.Houses).IsLoaded)
            {
                context.Entry(user).Collection(m => m.Houses).Load();
            }
            if (!context.Entry(user).Reference(m => m.Files).IsLoaded)
            {
                context.Entry(user).Reference(m => m.Files).Load();
            }
        }

        public string? LastName { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserAccess { get; set; } = string.Empty;
        public DateTime? BirthDay { get; set; } = DateTime.Now;
        public bool? Gender { get; set; } = false;
        public string? UrlImage { get; set; } = string.Empty;
        public int Point { get; set; } = 0;
        public double UserRating { get; set; } = 0;
        public int NumberOfHouses { get; set; } = 0;
        public int NumberRating { get; set; } = 0;
        public int NumberSwap { get; set; } = 0;
    }
}
