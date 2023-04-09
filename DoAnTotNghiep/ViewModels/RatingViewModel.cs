using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.ViewModels
{
    public class CreateRatingViewModel
    {
        public int? RatingHouse { get; set; }
        [Required(ErrorMessage = "Hãy đánh giá người dùng")]
        public int RatingUser { get; set; }
        [Required(ErrorMessage = "Hãy chọn yêu cầu đánh giá")]
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
    public class ListRating
    {
        public FrameRating OverView { get; set; } = new FrameRating();
        public List<DetailRatingWithUser> Rating { get; set; } = new List<DetailRatingWithUser>();
    }

    public class FrameRating
    {
        public int OneStar { get; set; } = 0;
        public int TwoStar { get; set; } = 0;
        public int ThreeStar { get; set; } = 0;
        public int FourStar { get; set; } = 0;
        public int FiveStar { get; set; } = 0;
        public int Total() => OneStar + TwoStar + ThreeStar + FourStar + FiveStar;
    }
}
