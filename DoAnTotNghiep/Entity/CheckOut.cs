using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Check_out")]
    public class CheckOut
    {
        [Key]
        [Column("id_user", Order = 1)]
        public int IdUser { get; set; }

        [Key]
        [Column("id_request", Order = 2)]
        public int IdRequest { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [ForeignKey(nameof(IdRequest))]
        public virtual Request? Requests { get; set; }
    }
}

