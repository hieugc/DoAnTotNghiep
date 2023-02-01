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
    }
}
