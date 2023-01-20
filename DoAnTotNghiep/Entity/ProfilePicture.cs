using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("profile_picture")]
    public class ProfilePicture
    {
        [Key]
        [Column("id_user", Order = 1)]
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User User { get; set; }

        [Key]
        [Column("id_file", Order = 1)]
        public int IdFile { get; set; }
        [ForeignKey("IdFile")]
        public File File { get; set; }
    }
}
