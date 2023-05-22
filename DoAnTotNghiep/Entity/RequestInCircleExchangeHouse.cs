using DoAnTotNghiep.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Request_in_circle_exchange_house")]
    public class RequestInCircleExchangeHouse
    {
        public void InCludeAll(DoAnTotNghiepContext context)
        {
            if (this.WaitingRequests == null && !context.Entry(this).Reference(m => m.WaitingRequests).IsLoaded)
                context.Entry(this).Reference(m => m.WaitingRequests).Load();
            if (this.CircleExchangeHouse == null && !context.Entry(this).Reference(m => m.CircleExchangeHouse).IsLoaded)
                context.Entry(this).Reference(m => m.CircleExchangeHouse).Load();
        }
        [Key]
        [Column("id_circle_exchange_house", Order = 1)]
        public int IdCircleExchangeHouse { get; set; }

        [Key]
        [Column("id_waiting_request", Order = 2)]
        public int IdWaitingRequest { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;

        [ForeignKey(nameof(IdCircleExchangeHouse))]
        public virtual CircleExchangeHouse? CircleExchangeHouse { get; set; }

        [ForeignKey(nameof(IdWaitingRequest))]
        public virtual WaitingRequest? WaitingRequests { get; set; }
    }
}
