using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }


        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("img_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Column("type")]
        public int Type { get; set; } = 0;

        [Required]
        [Column("id_type")]
        public int IdType { get; set; } = 0;

        [Required]
        [Column("created_date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("is_seen")]
        public bool IsSeen { get; set; } = false;


        [Column("id_user")]
        [Required]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }
    }
}
