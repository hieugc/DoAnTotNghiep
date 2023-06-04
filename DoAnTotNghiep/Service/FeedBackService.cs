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
    public class FeedBackService : IFeedBackService
    {
        private DoAnTotNghiepContext _context;
        public FeedBackService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public bool Save(FeedBack feedBack)
        {
            try
            {
                this._context.FeedBacks.Add(feedBack);
                this._context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "SaveHouse_IdUser_" + feedBack.Id);
            }
            return false;
        }
        public bool Update(FeedBack feedBack)
        {
            try
            {
                this._context.FeedBacks.Update(feedBack);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "UpdateFeedBack_IdUser_" + feedBack.Id);
            }
            return false;
        }
        public FeedBack? GetRatingById(int idUser, int idFeedBack) => this._context.FeedBacks.Where(m => m.Id == idFeedBack && m.IdUser == idUser).FirstOrDefault();
        public FeedBack? GetRatingById(int idFeedBack) => this._context.FeedBacks.Where(m => m.Id == idFeedBack).FirstOrDefault();
        public DetailRatingViewModel? GetRatingByRequestDiffUser(int idUser, int idRequest)
        {
            FeedBack? model = this._context.FeedBacks.Where(m => m.IdUser != idUser && m.IdRequest == idRequest)
                                    .FirstOrDefault();
            if (model == null) return null;
            model.IncludeAll(this._context);
            return new DetailRatingViewModel(model);
        }
        public DetailRatingViewModel? GetRatingByRequestDiffUser(ICollection<FeedBack>? feedBacks, int idUser)
        {
            if(feedBacks != null)
            {
                FeedBack? model = feedBacks.Where(m => m.IdUser != idUser).FirstOrDefault();
                if (model != null)
                {
                    model.IncludeAll(this._context);
                    return new DetailRatingViewModel(model);
                }
            }
            return null;
        }
        public DetailRatingViewModel? GetRatingByRequest(ICollection<FeedBack>? feedBacks, int idUser)
        {
            if (feedBacks != null)
            {
                FeedBack? model = feedBacks.Where(m => m.IdUser == idUser).FirstOrDefault();
                if (model != null)
                {
                    model.IncludeAll(this._context); return new DetailRatingViewModel(model);
                }
            }
            return null;
        }
        public List<DetailRatingWithUser> GetRatingByHouse(List<FeedBack> feedBacks, string host, byte[] salt)
        {
            List<DetailRatingWithUser> model = new List<DetailRatingWithUser>();
            foreach (var item in feedBacks)
            {
                item.IncludeAll(this._context);
                if (item.Users != null)
                {
                    DetailRatingViewModel rating = new DetailRatingViewModel(item);
                    UserInfo user = new UserInfo(item.Users, salt, host);
                    model.Add(new DetailRatingWithUser() { User = user, FeedBack = rating });
                }
            }
            return model;
        }
        public List<DetailRatingWithUser> GetRatingByHouse(House house, string host, byte[] salt)
        {
            List<DetailRatingWithUser> model = new List<DetailRatingWithUser>();
            if (house.FeedBacks != null)
            {
                model.AddRange(this.GetRatingByHouse(house.FeedBacks.ToList(), host, salt));
            }
            if (house.FeedBackOfCircles != null)
            {
                model.AddRange(this.GetRatingByHouse(house.FeedBackOfCircles.Select(m => new FeedBack().Create(m)).ToList(), host, salt));
            }
            return model;
        }
        public List<DetailRatingWithUser> GetRatingById(int IdAnotherUser, string host, byte[] salt)
        {
            return this._context.FeedBacks.Include(m => m.Users)
                .ThenInclude(u => u.Files).Where(m => m.IdUserRated == IdAnotherUser).
                Select(m => new DetailRatingWithUser(m, salt, host)).ToList();
        }
        public List<FeedBack> GetRatingByHouse(int IdHouse)
        {
            var model = this._context.FeedBacks.Where(m => m.IdHouse == IdHouse).ToList();
            var cmodel = this._context.FeedBackOfCircles.Where(m => m.IdHouse == IdHouse).Select(m => new FeedBack().Create(m)).ToList();
            model.AddRange(cmodel);
            return model.OrderByDescending(m => m.CreatedDate).ToList();
        }
        public List<FeedBack> All()
        {
            var model = this._context.FeedBacks.ToList();
            var cmodel = this._context.FeedBackOfCircles.Select(m => new FeedBack().Create(m)).ToList();
            model.AddRange(cmodel);
            return model.OrderByDescending(m => m.CreatedDate).ToList();
        }
    }

    public interface IFeedBackService
    {
        public bool Save(FeedBack feedBack);
        public bool Update(FeedBack feedBack);
        public FeedBack? GetRatingById(int idUser, int idFeedBack);
        public FeedBack? GetRatingById(int idFeedBack);
        public DetailRatingViewModel? GetRatingByRequestDiffUser(int idUser, int idRequest);
        public DetailRatingViewModel? GetRatingByRequestDiffUser(ICollection<FeedBack>? feedBacks, int idUser);
        public DetailRatingViewModel? GetRatingByRequest(ICollection<FeedBack>? feedBacks, int idUser);
        public List<FeedBack> GetRatingByHouse(int IdHouse);
        public List<FeedBack> All();
        public List<DetailRatingWithUser> GetRatingByHouse(List<FeedBack> feedBacks, string host, byte[] salt);
        public List<DetailRatingWithUser> GetRatingByHouse(House house, string host, byte[] salt);
        public List<DetailRatingWithUser> GetRatingById(int IdAnotherUser, string host, byte[] salt);

    }
}
