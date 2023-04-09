using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class TransactionViewModel{
        public TransactionViewModel(HistoryTransaction transaction) {
            this.Status = (transaction.Status == (int)StatusTransaction.USED);
            this.Content = transaction.Content;
            this.CreatedDate = transaction.CreatedDate;
            this.Amount = transaction.Amount;
        }
        public int Amount { get; set; } = 0;
        public bool Status { get; set; } = false; //TRUE => used || FALSE => PAYMENT
        public string? Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
