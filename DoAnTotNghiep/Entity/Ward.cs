using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Ward")]
    public class Ward
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên phường")]
        [MaxLength(100, ErrorMessage = "Tên phường tối đa 100 ký tự")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("lat")]
        public double Lat { get; set; } = 0;

        [Column("lng")]
        public double Lng { get; set; } = 0;

        [Column("id_district")]
        public virtual int? IdDistrict { get; set; }

        [ForeignKey(nameof(IdDistrict))]
        public virtual District? Districts { get; set; }
    }
}
