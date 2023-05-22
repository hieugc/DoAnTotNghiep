using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Job;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class CircleFeedBackService : ICircleFeedBackService
    {
        private DoAnTotNghiepContext _context;
        public CircleFeedBackService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void Save(FeedBackOfCircle feedBack)
        {
            this._context.FeedBackOfCircles.Add(feedBack);
            this._context.SaveChanges();
        }
        public void Update(FeedBackOfCircle feedBack)
        {
            this._context.FeedBackOfCircles.Update(feedBack);
            this._context.SaveChanges();
        }
        public FeedBackOfCircle? GetByCircleRequest(int idUser, int idFeedBack)
            => this._context.FeedBackOfCircles.Where(m => m.Id == idFeedBack && m.IdUserRated == idUser).FirstOrDefault();
        public FeedBackOfCircle? GetById(int idFeedBack) => this._context.FeedBackOfCircles.Where(m => m.Id == idFeedBack).FirstOrDefault();
    }

    public interface ICircleFeedBackService
    {
        public void Save(FeedBackOfCircle feedBack);
        public void Update(FeedBackOfCircle feedBack);
        public FeedBackOfCircle? GetByCircleRequest(int idUser, int idFeedBack);
        public FeedBackOfCircle? GetById(int idFeedBack);
    }
}
