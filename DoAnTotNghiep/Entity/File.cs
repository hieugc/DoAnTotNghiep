using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("File")]
    public class File
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("path_folder")]
        public string PathFolder { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;
    }
}
