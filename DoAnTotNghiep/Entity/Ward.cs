using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Entity
{
    public class Ward
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên phường")]
        [MaxLength(100, ErrorMessage = "Tên phường tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        public float Lat { get; set; } = 0;
        public float Long { get; set; } = 0;
        public int Count { get; set; } = 0;
    }
}
