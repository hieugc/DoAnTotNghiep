using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Request_in_circle_exchange_house")]
    public class RequestInCircleExchangeHouse
    {
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
