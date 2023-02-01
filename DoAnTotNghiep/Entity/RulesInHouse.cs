using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Rules_in_house")]
    public class RulesInHouse
    {
        [Key]
        [Column("id_rules", Order = 1)]
        public int IdRules { get; set; }

        [Key]
        [Column("id_house", Order = 2)]
        public int IdHouse { get; set; }

        [ForeignKey(nameof(IdRules))]
        public virtual Rules? Rules { get; set; }

        [ForeignKey(nameof(IdHouse))]
        public virtual House? Houses { get; set; }
    }
}
