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
    public class ReportService : IReportService
    {
        private DoAnTotNghiepContext _context;
        public ReportService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }

        public void Save(UserReport report)
        {
            this._context.UserReports.Add(report);
            this._context.SaveChanges();
        }
        public void Update(UserReport report)
        {
            this._context.UserReports.Update(report);
            this._context.SaveChanges();
        }
        public List<UserReport> GetByHouse(int idHouse) => this._context.UserReports
                                                                        .Include(m => m.Users)
                                                                        .Include(m => m.Files)
                                                                        .Where(m => m.IdHouse == idHouse).ToList();
        public List<UserReport> GetByUser(int idUser) => this._context.UserReports
                                                                        .Include(m => m.Users)
                                                                        .Include(m => m.Files)
                                                                        .Include(m => m.Houses)
                                                                        .Where(m => m.IdUserReport != null && m.IdUserReport == idUser || m.Houses != null && m.Houses.IdUser == idUser).ToList();
    }

    public interface IReportService
    {
        public void Save(UserReport report);
        public void Update(UserReport report);
        public List<UserReport> GetByHouse(int idHouse);
        public List<UserReport> GetByUser(int idUser);
    }
}
