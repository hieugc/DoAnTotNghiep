using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    public class House
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên căn nhà")]
        [MaxLength(100, ErrorMessage = "Tên nhà tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền giá căn nhà")]
        public int Price { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số người có thể ở")]
        public int People { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng ngủ")]
        public int BedRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng tắm")]
        public int BathRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền mô tả căn nhà")]
        public string DescriptionContent { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền diện tích căn nhà")]
        public float Area { get; set; } = 0;

        public int NumberOfRating { get; set; } = 0;
        public float Rating { get; set; } = 0;
        public int Status { get; set; } = 0;
        public float Lat { get; set; } = 0;
        public float Long { get; set; } = 0;


        [ForeignKey("Id")]
        public City? City { get; set; }
        [ForeignKey("Id")]
        public District? District { get; set; }
        [ForeignKey("Id")]
        public Ward? Ward { get; set; }
    }
}
