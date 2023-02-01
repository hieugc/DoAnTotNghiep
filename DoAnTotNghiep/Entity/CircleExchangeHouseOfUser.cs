using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Circle_exchange_house_of_user")]
    public class CircleExchangeHouseOfUser
    {
        [Key]
        [Column("id_circle_exchange_house", Order = 1)]
        public int IdCircleExchangeHouse { get; set; }

        [Key]
        [Column("id_user", Order = 2)]
        public int IdUser { get; set; }

        [ForeignKey(nameof(IdCircleExchangeHouse))]
        public virtual CircleExchangeHouse? CircleExchangeHouse { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }
    }
}
