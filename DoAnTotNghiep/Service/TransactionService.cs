using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class TransactionService : ITransactionService
    {
        private DoAnTotNghiepContext _context;
        public TransactionService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void Save(HistoryTransaction historyTransaction)
        {
            this._context.HistoryTransactions.Add(historyTransaction);
            this._context.SaveChanges();
        }
        public void Update(HistoryTransaction historyTransaction)
        {
            this._context.HistoryTransactions.Update(historyTransaction);
            this._context.SaveChanges();
        }
        public HistoryTransaction? GetById(int id)
        {
            return this._context.HistoryTransactions.Where(m => m.Id == id).FirstOrDefault();
        }
        public List<TransactionViewModel> GetTransaction(int? Status, int IdUser, int year = 2023)
        {
            List<TransactionViewModel> transaction = new List<TransactionViewModel>();
            if (Status.HasValue)
            {
                transaction.AddRange(this._context.HistoryTransactions
                                            .Where(m => m.Status == Status && m.IdUser == IdUser && m.CreatedDate.Year == year)
                                            .OrderBy(m => m.CreatedDate)
                                            .Select(m => new TransactionViewModel(m))
                                            .ToList());
            }
            else
            {
                transaction.AddRange(this._context.HistoryTransactions
                                            .Where(m => m.IdUser == IdUser && m.CreatedDate.Year == year)
                                            .OrderBy(m => m.CreatedDate)
                                            .Select(m => new TransactionViewModel(m))
                                            .ToList());
            }
            return transaction;
        }
    }

    public interface ITransactionService
    {
        public void Save(HistoryTransaction historyTransaction);
        public void Update(HistoryTransaction historyTransaction);
        public HistoryTransaction? GetById(int id);
        public List<TransactionViewModel> GetTransaction(int? Status, int IdUser, int year = 2023);
    }
}
