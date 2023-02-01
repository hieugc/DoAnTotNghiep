using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Request")]
    public class Request
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("start_date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("status")]
        public int Status { get; set; } = 0;

        [Required]
        [Column("point")]
        public int Point { get; set; } = 0;

        [Required]
        [Column("type")]
        public int Type { get; set; }

        [Required]
        [Column("created_date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("updated_date")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        [Column("id_user")]
        [Required]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Column("id_house")]
        [Required]
        public virtual int IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get;set; }

        [Column("id_swap_house")]
        public virtual int? IdSwapHouse { get; set; }

        [ForeignKey(nameof(IdSwapHouse))]
        public virtual House? SwapHouses { get; set; }
    }
}
