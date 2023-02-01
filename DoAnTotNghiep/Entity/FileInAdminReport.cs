using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("File_in_admin_report")]
    public class FileInAdminReport
    {
        [Key]
        [Column("id_file", Order = 1)]
        public int IdFile { get; set; }

        [Key]
        [Column("id_admin_report", Order = 2)]
        public int IdAdminReport { get; set; }

        [ForeignKey(nameof(IdFile))]
        public virtual File? Files { get; set; }
        [ForeignKey(nameof(IdAdminReport))]
        public virtual AdminReport? AdminReports { get; set; }
    }
}
