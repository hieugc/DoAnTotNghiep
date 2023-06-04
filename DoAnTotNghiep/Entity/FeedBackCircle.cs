using DoAnTotNghiep.Data;
using DoAnTotNghiep.ViewModels;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Feed_back_of_circle")]
    public class FeedBackOfCircle
    {
        public FeedBackOfCircle Create(CreateRatingViewModel feedBack, int IdUserRated, int IdUserRating, int IdHouse)
        {
            DateTime now = DateTime.Now;
            return new FeedBackOfCircle()
            {
                IdCircle = feedBack.IdRequest,
                Content = feedBack.Content,
                RateHouse = (feedBack.RatingHouse == null? 0: feedBack.RatingHouse.Value),
                RateUser = feedBack.RatingUser,
                IdUserRated = IdUserRated,
                IdUserRating = IdUserRating,
                IdHouse = IdHouse,
                CreatedDate = now,
                UpdatedDate = now
            };
        }

        public void Update(EditRatingViewModel feedBack)
        {
            this.Content = feedBack.Content;
            this.RateHouse = (feedBack.RatingHouse == null ? 0: feedBack.RatingHouse.Value);
            this.RateUser = feedBack.RatingUser;
            this.UpdatedDate = DateTime.Now;
        }


        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if (this.UserRating == null && !context.Entry(this).Reference(m => m.UserRating).IsLoaded)
                context.Entry(this).Reference(m => m.UserRating).Load();
            if (this.UserRated == null && !context.Entry(this).Reference(m => m.UserRated).IsLoaded)
                context.Entry(this).Reference(m => m.UserRated).Load();
            if (this.Houses == null && !context.Entry(this).Reference(m => m.Houses).IsLoaded)
                context.Entry(this).Reference(m => m.Houses).Load();
        }


        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("content")]
        [MaxLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        public string Content { get; set; } = string.Empty;
        [Required]
        [Column("rate_user")]
        public int RateUser { get; set; } = 0;

        [Required]
        [Column("rate_house")]
        public int RateHouse { get; set; } = 0;

        [Required]
        [Column("created_date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("updated_date")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        [Column("id_user_rating")]
        [Required]
        public virtual int IdUserRating { get; set; } //người đánh giá

        [ForeignKey(nameof(IdUserRating))]
        public virtual User? UserRating { get; set; }

        [Column("id_house")]
        public virtual int? IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get;set; }

        [Required]
        [Column("id_circle")]
        public virtual int IdCircle { get; set; }

        [ForeignKey(nameof(IdCircle))]
        public virtual CircleExchangeHouse? CircleExchangeHouse { get; set; }

        [Required]
        [Column("id_user_rated")]
        public virtual int IdUserRated { get; set; }// được đánh giá

        [ForeignKey(nameof(IdUserRated))]
        public virtual User? UserRated { get; set; }
    }
}
