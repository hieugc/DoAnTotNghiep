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
    public class NotificationService : INotificationService
    {
        private DoAnTotNghiepContext _context;
        public NotificationService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void SaveNotification(Notification notification)
        {
            this._context.Notifications.Add(notification);
            this._context.SaveChanges();
        }
        public void SeenAll(int idUser)
        {
            var model = this._context.Notifications
                                        .Where(m => m.IdUser == idUser)
                                        .ToList();
            try
            {
                foreach (var item in model) item.IsSeen = true;
                this._context.Notifications.UpdateRange(model);
                this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void Seen(int idUser, int id)
        {
            var model = this._context.Notifications
                                        .Where(m => m.IdUser == idUser && m.Id == id)
                                        .ToList();
            try
            {
                foreach (var item in model) item.IsSeen = true;
                this._context.Notifications.UpdateRange(model);
                this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public List<NotificationViewModel> GetByUser(int idUser, string host)
        {
            return this._context.Notifications
                                    .OrderByDescending(m => m.CreatedDate)
                                    .Where(m => m.IdUser == idUser)
                                    .Select(m => new NotificationViewModel(m, host))
                                    .ToList();
        }
    }

    public interface INotificationService
    {
        public void SaveNotification(Notification notification);
        public void SeenAll(int idUser);
        public void Seen(int idUser, int id);
        public List<NotificationViewModel> GetByUser(int idUser, string host);
    }
}
