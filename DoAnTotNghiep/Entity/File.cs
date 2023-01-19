using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string PathFolder { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;
    }
}
