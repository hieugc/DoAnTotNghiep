using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Entity
{
    public class District
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên quận")]
        [MaxLength(100, ErrorMessage = "Tên quận tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        public float Lat { get; set; } = 0;//địa điểm trung tâm
        public float Long { get; set; } = 0;//địa điểm trung tâm
        public int Count { get; set; } = 0;
    }
}
