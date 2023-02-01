using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Feed_back")]
    public class FeedBack
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("content")]
        [MaxLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Column("rating")]
        public int Rating { get; set; } = 0;

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

        [Column("id_request")]
        public virtual int? IdRequest { get; set; }

        [ForeignKey(nameof(IdRequest))]
        public virtual Request? Requests { get; set; }
    }
}
