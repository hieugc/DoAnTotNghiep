using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Rules")]
    public class Rules
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền nội dung quy tắc")]
        [MaxLength(100, ErrorMessage = "Quy tắc tối đa 100 ký tự")]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("icon")]
        public string Icon { get; set; } = string.Empty;
    }
}
