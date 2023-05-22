using DoAnTotNghiep.Data;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
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

        public void CheckStatus(Request request, int IdUser)
        {
            if (request.CheckOuts != null && request.CheckOuts.Any(m => m.IdUser == IdUser) && request.Status == (int)StatusRequest.CHECK_IN)
            {
                this.Status = (int)StatusRequest.CHECK_OUT;
            }
            else if (request.CheckIns != null && request.CheckIns.Any(m => m.IdUser == IdUser) && request.Status == (int)StatusRequest.ACCEPT)
            {
                this.Status = (int)StatusRequest.CHECK_IN;
            }
            else if (request.FeedBacks != null && request.FeedBacks.Any(m => m.IdUser == IdUser) && request.Status == (int)StatusRequest.CHECK_OUT)
            {
                this.Status = (int)StatusRequest.ENDED;
            }
        }
        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if(this.Houses == null && !context.Entry(this).Reference(m => m.Houses).IsLoaded)
                context.Entry(this).Reference(m => m.Houses).Load();
            if (this.FeedBacks == null && !context.Entry(this).Collection(m => m.FeedBacks).IsLoaded)
                context.Entry(this).Collection(m => m.FeedBacks).Load();
            if (this.CheckOuts == null && !context.Entry(this).Collection(m => m.CheckOuts).IsLoaded)
                context.Entry(this).Collection(m => m.CheckOuts).Load();
            if (this.CheckIns == null && !context.Entry(this).Collection(m => m.CheckIns).IsLoaded)
                context.Entry(this).Collection(m => m.CheckIns).Load();
            if (this.Users == null && !context.Entry(this).Reference(m => m.Users).IsLoaded)
                context.Entry(this).Reference(m => m.Users).Load();
            if (this.IdSwapHouse != null)
            {
                if (this.SwapHouses == null && !context.Entry(this).Reference(m => m.SwapHouses).IsLoaded)
                    context.Entry(this).Reference(m => m.SwapHouses).Load();
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
