using DoAnTotNghiep.Data;
using DoAnTotNghiep.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Feed_back")]
    public class FeedBack
    {
        public void Create(CreateRatingViewModel model, int IdUser, int IdUserRated, int? IdHouse)
        {
            this.UpdatedDate = DateTime.Now;
            this.CreatedDate = DateTime.Now;
            this.Content = model.Content;
            this.Rating = model.RatingHouse == null? 0: model.RatingHouse.Value;
            this.RatingUser = model.RatingUser;
            this.IdHouse = IdHouse;
            this.IdRequest = model.IdRequest;
            this.IdUserRated = IdUserRated;
            this.IdUser = IdUser;
        }

        public void Update(EditRatingViewModel model)
        {
            this.UpdatedDate = DateTime.Now;
            this.Content = model.Content;
            this.Rating = model.RatingHouse == null ? 0 : model.RatingHouse.Value;
            this.RatingUser = model.RatingUser;
        }

        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if (this.Users == null && !context.Entry(this).Reference(m => m.Users).IsLoaded)
                context.Entry(this).Reference(m => m.Users).Load();
            if (this.UserRated == null && !context.Entry(this).Reference(m => m.UserRated).IsLoaded)
                context.Entry(this).Reference(m => m.UserRated).Load();
            if (this.Houses == null && !context.Entry(this).Reference(m => m.Houses).IsLoaded)
                context.Entry(this).Reference(m => m.Houses).Load();
        }

        public FeedBack Create(FeedBackOfCircle feedBack)
        {
            return new FeedBack()
            {
                Id = feedBack.Id,
                Content = feedBack.Content,
                Rating = feedBack.RateHouse,
                RatingUser = feedBack.RateUser,
                CreatedDate = feedBack.CreatedDate,
                UpdatedDate = feedBack.UpdatedDate,
                IdUser = feedBack.IdUserRating,
                IdUserRated = feedBack.IdUserRated,
                IdHouse = feedBack.IdHouse,
            };
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("content")]
        [MaxLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Column("rating")]
        public int Rating { get; set; } = 0;

        [Required]
        [Column("user_rating")]
        public int RatingUser { get; set; } = 0;

        [Required]
        [Column("created_date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("updated_date")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        [Column("id_user")]
        [Required]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }//người đánh giá

        [Column("id_house")]
        public virtual int? IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get;set; }

        [Required]
        [Column("id_request")]
        public virtual int IdRequest { get; set; }

        [ForeignKey(nameof(IdRequest))]
        public virtual Request? Requests { get; set; }

        [Required]
        [Column("id_user_rated")]
        public virtual int IdUserRated { get; set; }

        [ForeignKey(nameof(IdUserRated))]
        public virtual User? UserRated { get; set; }//đc đánh giá
    }
}
