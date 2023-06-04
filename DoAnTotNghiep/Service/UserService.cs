using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class UserService : IUserService
    {
        private DoAnTotNghiepContext _context;
        public UserService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public User? GetByEmail(string email)
        {
            return this._context.Users.Where(m => m.Email== email && m.IsBan == false).FirstOrDefault();
        }
        public User? GetById(int Id)
        {
            return this._context.Users.Where(m => m.Id == Id && m.IsBan == false).FirstOrDefault();
        }
        public bool IsExistEmail(string email)
        {
            return this._context.Users.Where(m => m.Email == email).Any();
        }
        public bool IsExistEmail(string email, int id) => this._context.Users.Any(m => m.Email == email && m.Id != id);
        public bool SaveUser(User user)
        {
            try
            {
                this._context.Users.Add(user);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "SaveUser_" + user.Email);
            }
            return false;
        }

        public bool UpdateUser(User user)
        {
            try
            {
                this._context.Users.Update(user);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "UpdateUser_" + user.Email);
            }
            return false;
        }
        public int NumberSwap(int id)
        {
            var numberSwap = from u in this._context.Users
                             join h in this._context.Houses on u.Id equals h.IdUser
                             join rq in this._context.Requests on h.Id equals rq.IdHouse
                             where u.Id == id && rq.Status >= (int)StatusRequest.CHECK_IN
                             select rq;
            return (numberSwap == null ? 0 : numberSwap.ToList().Count());
        }
        public List<int> CalRate(int idUser)
        {
            int number = 0;
            int value = 0;
            var rate = this._context.FeedBacks.Where(m => m.IdUserRated == idUser).ToList();
            foreach (var item in rate.ToList())
            {
                value += item.RatingUser;
            }
            number += rate.ToList().Count();
            var crate = this._context.FeedBackOfCircles.Where(m => m.IdUserRated == idUser).ToList();
            foreach (var item in crate)
            {
                value += item.RateUser;
            }
            number += crate.ToList().Count();
            return new List<int>() { value, number <= 0 ? 1 : number };
        }
        public List<User> GetByCircle(int idCircle)
        {
            List<User> users = new List<User>();

            var item = from cru in this._context.CircleExchangeHouseOfUsers
                       join u in this._context.Users on cru.IdUser equals u.Id
                       where cru.IdCircleExchangeHouse == idCircle
                       select u;
            if(item != null ) users.AddRange(item.ToList());
            return users;
        }
        public List<User> All()
        {
            return this._context.Users.Where(m => m.Role != (int)Role.AdminCode && m.IsBan == false).ToList();
        }
        public void Demo(IConfiguration configuration)
        {
            //thêm người
            List<User> users = new List<User>();
            List<string> firstName = new List<string>() { "Kim", "Ánh", "Hào", "Tài" };
            List<string> lastName = new List<string>() { "Hàn", "Hồng", "Nhật", "Phát" };
            List<string> emails = new List<string>() {
                "kimhan@gmail.com",
                "anhhong@gmail.com",
                "haonhat@gmail.com",
                "abcc@gmail.com"
            };
            for (var item = 0; item < 4; item++)
            {
                byte[] salt = Crypto.Salt();
                int point = item < 3 ? 0 : 2000;
                User user = new User()
                {
                    FirstName = firstName.ElementAt(item),
                    LastName = lastName.ElementAt(item),
                    Gender = true,
                    BirthDay = new DateTime(2001, 01, 01),
                    Email = emails.ElementAt(item),
                    Password = Crypto.HashPass("Vinova123", salt),
                    Salt = Crypto.SaltStr(salt),
                    PhoneNumber = "0973409127",
                    Point = point,
                    BonusPoint = 0,
                    IdFile = null,
                    UserRating = 0,
                    Role = Role.MemberCode
                };
                users.Add(user);
            }
            this._context.Users.AddRange(users);
            this._context.SaveChanges();
        }
    }

    public interface IUserService
    {
        public User? GetByEmail(string email);//email => unique
        public User? GetById(int Id);
        public bool IsExistEmail(string email);
        public bool IsExistEmail(string email, int id);
        public bool SaveUser(User user);
        public bool UpdateUser(User user);
        public int NumberSwap(int id);
        public List<int> CalRate(int idUser);
        public List<User> GetByCircle(int idCircle);
        public List<User> All();
        public void Demo(IConfiguration configuration);
    }
}
