using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("House")]
    public class House
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên căn nhà")]
        [MaxLength(100, ErrorMessage = "Tên nhà tối đa 100 ký tự")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("status")]
        public int Status { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền giá căn nhà")]
        [Column("price")]
        public int Price { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số người có thể ở")]
        [Column("people")]
        public int People { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng ngủ")]
        [Column("bedroom")]
        public int BedRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng tắm")]
        [Column("bathroom")]
        public int BathRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền mô tả căn nhà")]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền diện tích căn nhà")]
        [Column("area")]
        public double Area { get; set; } = 0;

        [Column("number_of_rating")]
        public int NumberOfRating { get; set; } = 0;

        [Column("rating")]
        public double Rating { get; set; } = 0;
        [Column("lat")]
        public double Lat { get; set; } = 0;
        [Column("lng")]
        public double Lng { get; set; } = 0;

        [Column("id_user")]
        public virtual int? IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Column("id_city")]
        public virtual int? IdCity { get; set; }
        [ForeignKey(nameof(IdCity))]
        public virtual City? Citys { get; set; }

        [Column("id_district")]
        public virtual int? IdDistrict { get; set; }
        [ForeignKey(nameof(IdDistrict))]
        public virtual District? Districts { get; set; }
        [Column("id_ward")]
        public virtual int? IdWard { get; set; }
        [ForeignKey(nameof(IdWard))]
        public virtual Ward? Wards { get; set; }
    }
}
