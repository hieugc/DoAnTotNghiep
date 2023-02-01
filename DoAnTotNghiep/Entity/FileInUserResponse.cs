using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("File_in_user_response")]
    public class FileInUserResponse
    {
        [Key]
        [Column("id_file", Order = 1)]
        public int IdFile { get; set; }

        [Key]
        [Column("id_user_response", Order = 2)]
        public int IdUserResponse { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }
        [ForeignKey(nameof(IdUserResponse))]
        public virtual UserResponse? UserResponses { get; set; }
    }
}
