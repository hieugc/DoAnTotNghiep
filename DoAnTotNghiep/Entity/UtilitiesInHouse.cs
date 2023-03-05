using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Utilities_in_house")]
    public class UtilitiesInHouse
    {
        [Key]
        [Column("id_utilities", Order = 1)]
        public int IdUtilities { get; set; }

        [Key]
        [Column("id_house", Order = 2)]
        public int IdHouse { get; set; }
        [Column("status")]
        public bool Status { get; set; } = false;

        [ForeignKey(nameof(IdUtilities))]
        public virtual Utilities? Utilities { get; set; }

        [ForeignKey(nameof(IdHouse))]
        public virtual House? Houses { get; set; }
    }
}
