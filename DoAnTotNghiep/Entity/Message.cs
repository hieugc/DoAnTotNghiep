using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Message")]
    public class Message
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền nội dung câu hỏi")]
        [MaxLength(200, ErrorMessage = "Nội dung câu hỏi tối đa 100 ký tự")]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("id_user")]
        public virtual int? IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Column("id_chat_room")]
        [Required]
        public virtual int IdChatRoom { get; set; }

        [ForeignKey(name: nameof(IdChatRoom))]
        public virtual ChatRoom? ChatRooms { get; set; }

        [Column("id_reply")]
        [Required]
        public virtual int IdReply { get; set; }

        [ForeignKey(name: nameof(IdReply))]
        public virtual Message? Messages { get; set; }
    }
}
