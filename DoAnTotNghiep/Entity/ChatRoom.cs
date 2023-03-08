using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Chat_room")]
    public class ChatRoom
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<UsersInChatRoom>? UsersInChatRooms { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
