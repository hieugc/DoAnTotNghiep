using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("AdminReport")]
    public class AdminReport
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền nội dung câu hỏi")]
        [MaxLength(1000, ErrorMessage = "Nội dung câu hỏi tối đa 100 ký tự")]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("deadline_date")]
        public DateTime DeadlineDate { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("id_user")]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Column("id_house")]
        [Required]
        public virtual int IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual ICollection<FileInAdminReport>? Files { get; set; }
    }
}
