using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Request")]
    public class Request
    {
        public void UpdateRequest(EditRequestViewModel model)
        {
            this.UpdatedDate = DateTime.Now;
            this.Point = model.Price;
            this.Type = model.Type;
            this.IdHouse = model.IdHouse;
            this.IdSwapHouse = model.IdSwapHouse;
            this.StartDate = model.StartDate;
            this.EndDate = model.EndDate;
        }

        public void CheckStatus(Request request)
        {

            if (request.CheckOuts != null && request.CheckOuts.Count() > 0 && request.Status == (int)StatusRequest.CHECK_IN)
            {
                this.Status = (int)StatusRequest.CHECK_OUT;
            }
            else if (request.CheckIns != null && request.CheckIns.Count() > 0 && request.Status == (int)StatusRequest.ACCEPT)
            {
                this.Status = (int)StatusRequest.CHECK_IN;
            }
            else if (request.FeedBacks != null && request.FeedBacks.Count() > 0 && request.Status == (int)StatusRequest.CHECK_OUT)
            {
                this.Status = (int)StatusRequest.ENDED;
            }
        }


        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("start_date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("status")]
        public int Status { get; set; } = 0;

        [Required]
        [Column("point")]
        public int Point { get; set; } = 0;

        [Required]
        [Column("type")]
        public int Type { get; set; }

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
        public virtual User? Users { get; set; }

        [Column("id_house")]
        [Required]
        public virtual int IdHouse { get; set; }

        [ForeignKey(name: nameof(IdHouse))]
        public virtual House? Houses { get;set; }

        [Column("id_swap_house")]
        public virtual int? IdSwapHouse { get; set; }

        [ForeignKey(nameof(IdSwapHouse))]
        public virtual House? SwapHouses { get; set; }
        public ICollection<FeedBack>? FeedBacks { get; set; }
        public ICollection<CheckOut>? CheckOuts { get; set; }
        public ICollection<CheckIn>? CheckIns { get; set; }
    }
}
