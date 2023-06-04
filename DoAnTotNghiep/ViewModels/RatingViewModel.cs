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
        public EditRatingViewModel(){}
        public EditRatingViewModel(FeedBack feed)
        {
            this.Id = feed.Id;
            this.RatingHouse = feed.Rating;
            this.RatingUser = feed.RatingUser;
            this.Content = feed.Content;
            this.IdRequest = feed.IdRequest;
        }
        public EditRatingViewModel(FeedBackOfCircle feed)
        {
            this.Id = feed.Id;
            this.RatingHouse = feed.RateHouse;
            this.RatingUser = feed.RateUser;
            this.Content = feed.Content;
            this.IdRequest = feed.IdCircle;
        }
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
            if(feedBack.Users != null)
            {
                this.UserName = feedBack.Users.FirstName + " " + feedBack.Users.LastName;
            }
            this.IdRequest = feedBack.IdRequest;
        }

        public DetailRatingViewModel(FeedBackOfCircle feedBack, int IdUser)
        {
            this.CreatedDate = feedBack.CreatedDate;
            this.UpdatedDate = feedBack.UpdatedDate;
            this.Content = feedBack.Content;
            this.Rating = feedBack.RateHouse;
            this.RatingUser = feedBack.RateUser;
            this.Id = feedBack.Id;
            if (feedBack.UserRating != null)
            {
                this.UserName = feedBack.UserRating.FirstName + " " + feedBack.UserRating.LastName;
            }
            this.IsOwner = IdUser == feedBack.IdUserRating;
            this.IdRequest = feedBack.IdCircle;
        }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public int RatingUser { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public int IdRequest { get; set; } = 0;
        public bool IsOwner { get; set; } = false;
    }

    public class DetailRatingWithUser
    {
        public DetailRatingWithUser() { }

        public DetailRatingWithUser(FeedBack feedBack, byte[] salt, string host) { 
            this.FeedBack = new DetailRatingViewModel(feedBack);
            if(feedBack.Users!= null)
            {
                this.User = new UserInfo(feedBack.Users, salt, host);
            }
        }
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
        public double Rating() => (double) (OneStar + TwoStar*2 + ThreeStar*3 + FourStar*4 + FiveStar*5)/Total();
    }
}
