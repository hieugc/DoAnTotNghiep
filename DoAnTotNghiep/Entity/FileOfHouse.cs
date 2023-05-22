using DoAnTotNghiep.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("File_of_house")]
    public class FileOfHouse
    {
        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if (this.Houses == null && !context.Entry(this).Reference(m => m.Houses).IsLoaded)
                context.Entry(this).Reference(m => m.Houses).Load();
            if (this.Files == null && !context.Entry(this).Reference(m => m.Files).IsLoaded)
                context.Entry(this).Reference(m => m.Files).Load();
        }
        [Key]
        [Column("id_file", Order = 1)]
        public int IdFile { get; set; }

        [Key]
        [Column("id_house", Order = 2)]
        public int IdHouse { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }
        [ForeignKey(nameof(IdHouse))]
        public virtual House? Houses { get; set; }
    }
}
