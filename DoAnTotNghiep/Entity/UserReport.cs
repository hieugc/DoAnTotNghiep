using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("UserReport")]
    public class UserReport
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
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }


        [Column("is_responsed")]
        public bool IsResponsed { get; set; } = false;


        [Column("id_house")]
        [Required]
        public virtual int IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get; set; }

        public ICollection<FileInUserReport>? Files { get; set; }
    }
}
