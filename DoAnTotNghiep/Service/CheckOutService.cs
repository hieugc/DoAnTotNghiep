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
    public class CheckOutService : ICheckOutService
    {
        private DoAnTotNghiepContext _context;
        public CheckOutService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void Save(CheckOut CheckOut)
        {
            this._context.CheckOuts.Add(CheckOut);
            this._context.SaveChanges();
        }
        public void Update(CheckOut CheckOut)
        {
            this._context.CheckOuts.Update(CheckOut);
            this._context.SaveChanges();
        }
    }

    public interface ICheckOutService
    {
        public void Save(CheckOut CheckOut);
        public void Update(CheckOut CheckOut);
    }
}
