using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Users_in_chat_room")]
    public class UsersInChatRoom
    {
        [Key]
        [Column("id_user", Order = 1)]
        public int IdUser { get; set; }

        [Key]
        [Column("id_chat_room", Order = 2)]
        public int IdChatRoom { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [ForeignKey(nameof(IdChatRoom))]
        public virtual ChatRoom? ChatRooms { get; set; }
    }
}
