using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class CreateFeedBack
    {
        [Required(ErrorMessage = "Hãy điền cảm nhận của bạn về căn nhà")]
        [MaxLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá không hợp lệ")]
        public int Rating { get; set; } = 0;

        [Required(ErrorMessage = "Hãy thêm nhà đã giao dịch")]
        public int IdHouse { get; set; }

        [Required(ErrorMessage = "Hãy thêm yêu cầu đã giao dịch")]
        public int IdRequest { get; set; }
    }
}
