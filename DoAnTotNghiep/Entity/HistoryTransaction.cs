using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("History_transaction")]
    public class HistoryTransaction
    {
        public HistoryTransaction() { }

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
