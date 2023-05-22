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
    public class CheckInService : ICheckInService
    {
        private DoAnTotNghiepContext _context;
        public CheckInService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void Save(CheckIn checkIn)
        {
            this._context.CheckIns.Add(checkIn);
            this._context.SaveChanges();
        }
        public void Update(CheckIn checkIn)
        {
            this._context.CheckIns.Update(checkIn);
            this._context.SaveChanges();
        }
    }

    public interface ICheckInService
    {
        public void Save(CheckIn checkIn);
        public void Update(CheckIn checkIn);
    }
}
