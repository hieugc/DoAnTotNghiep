using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.ViewModels
{
    public class CreateRatingViewModel
    {
        [Required(ErrorMessage = "Hãy điền nội dung")]
        public int RatingHouse { get; set; }
        [Required(ErrorMessage = "Hãy điền nội dung")]
        public int RatingUser { get; set; }
        [Required(ErrorMessage = "Hãy điền nội dung")]
        public int IdRequest { get; set; }
        [Required(ErrorMessage = "Hãy điền nội dung")]
        [MaxLength(1000, ErrorMessage = "Nội dung tối đa 500 ký tự")]
        public string Content { get; set; } = string.Empty;
    }
    public class EditRatingViewModel: CreateRatingViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class DetailRatingViewModel
    {
        public DetailRatingViewModel()
        {

        }
        public DetailRatingViewModel(FeedBack feedBack)
        {
            this.CreatedDate = feedBack.CreatedDate;
            this.UpdatedDate = feedBack.UpdatedDate;
            this.Content = feedBack.Content;
            this.Rating = feedBack.Rating;
            this.RatingUser = feedBack.RatingUser;
            this.Id = feedBack.Id;
        }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public int RatingUser { get; set; } = 0;
        public int Id { get; set; } = 0;
    }

    public class DetailRatingWithUser
    {
        public UserInfo User { get; set; } = new UserInfo();
        public DetailRatingViewModel FeedBack { get; set; } = new DetailRatingViewModel();
    }
    public class DetailRatingWithHouse
    {
        public DetailHouseViewModel House { get; set; } = new DetailHouseViewModel();
        public DetailRatingViewModel FeedBack { get; set; } = new DetailRatingViewModel();
    }
}
