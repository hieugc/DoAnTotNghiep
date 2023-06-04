using DoAnTotNghiep.Data;
using DoAnTotNghiep.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("History_transaction")]
    public class HistoryTransaction
    {
        public HistoryTransaction() { }

        public HistoryTransaction RequestTransaction(Request request, User user, int idUser) 
        { 
            return new HistoryTransaction()
            {
                Amount = request.Point,
                IdUser = idUser,
                CreatedDate = DateTime.Now,
                Status = (int)StatusTransaction.USED,
                Content = "Bạn thanh toán "
                            + request.Point
                            + " điểm khi trao đổi nhà "
                            + request.Houses.Name + " của "
                            + user.LastName + " " + user.FirstName
                            + " vào lúc" + DateTime.Now.ToString("hh:mm:ss dd/MM/yyyy")
            };
        }

        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if (this.Users == null && !context.Entry(this).Reference(m => m.Users).IsLoaded)
                context.Entry(this).Reference(m => m.Users).Load();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("amount")]
        public int Amount { get; set; } = 0;

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("content")]
        [MaxLength(1000)]
        public string? Content { get; set; } = string.Empty;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("id_user")]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }
    }
}
