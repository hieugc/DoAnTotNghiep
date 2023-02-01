using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("District")]
    public class District
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên quận")]
        [MaxLength(100, ErrorMessage = "Tên quận tối đa 100 ký tự")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("lat")]
        public double Lat { get; set; } = 0;//địa điểm trung tâm

        [Column("long")]
        public double Long { get; set; } = 0;//địa điểm trung tâm

        [Column("count")]
        public int Count { get; set; } = 0;

        [Column("id_city")]
        public virtual int? IdCity { get; set; }

        [ForeignKey(nameof(IdCity))]
        public virtual City? Citys { get; set; } 
    }
}
