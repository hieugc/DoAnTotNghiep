using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Files_in_user_report")]
    public class FileInUserReport
    {
        [Key]
        [Column("id_file", Order = 1)]
        public int IdFile { get; set; }

        [Key]
        [Column("id_user_report", Order = 2)]
        public int IdUserReport { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }
        [ForeignKey(nameof(IdUserReport))]
        public virtual UserReport? UserReports { get; set; }
    }
}
