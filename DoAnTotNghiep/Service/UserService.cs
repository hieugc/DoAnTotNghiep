using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
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
            return this._context.Users.Where(m => m.Email== email).FirstOrDefault();
        }
        public User? GetById(int Id)
        {
            return this._context.Users.Where(m => m.Id == Id).FirstOrDefault();
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
            var rate = from u in this._context.Users
                       join f in this._context.FeedBacks on u.Id equals f.IdUserRated
                       where u.Id == idUser
                       select f;
            if(rate != null)
            {
                int value = 0;
                foreach(var item in rate.ToList())
                {
                    value += item.RatingUser;
                }
                int number = rate.ToList().Count();
                return new List<int>() { value, number <= 0? 1: number };
            }
            return new List<int>() { 0, 1 };
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
    }
}
