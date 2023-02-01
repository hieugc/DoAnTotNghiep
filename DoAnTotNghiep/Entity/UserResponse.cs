using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("UserResponse")]
    public class UserResponse
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền nội dung câu hỏi")]
        [MaxLength(1000, ErrorMessage = "Nội dung câu hỏi tối đa 100 ký tự")]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("id_user")]
        public virtual int? IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Column("id_admin_report")]
        [Required]
        public virtual int IdAdminReport { get; set; }

        [ForeignKey(name: nameof(IdAdminReport))]
        public virtual AdminReport? AdminReports { get; set; }
    }
}
