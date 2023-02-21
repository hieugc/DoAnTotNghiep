using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("City")]
    public class City
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên thành phố")]
        [MaxLength(100, ErrorMessage = "Tên thành phố tối đa 100 ký tự")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("lat")]
        public double Lat { get; set; } = 0;

        [Column("lng")]
        public double Lng { get; set; } = 0;

        [Column("count")]
        public int Count { get; set; } = 0;
    }
}
