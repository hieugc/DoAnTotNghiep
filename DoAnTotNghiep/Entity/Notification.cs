using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Notification")]
    public class Notification
    {
        public Notification() { }
        public Notification DemoNotification(int IdUser)
        {
            Notification notification = new Notification()
            {
                Title = NotificationType.DemoTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.DEMO,
                IdUser = IdUser,
                IdType = 0,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert
            };
            return notification;
        }

        public Notification Request(Request request)
        {
            Notification notification = new Notification()
            {
                Title = NotificationType.RequestTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = request.IdUser,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert
            };
            return notification;
        }


        [Key]
        [Column("id")]
        public int Id { get; set; }


        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("img_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Column("type")]
        public int Type { get; set; } = 0;

        [Required]
        [Column("id_type")]
        public int IdType { get; set; } = 0;

        [Required]
        [Column("created_date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("is_seen")]
        public bool IsSeen { get; set; } = false;


        [Column("id_user")]
        [Required]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }
    }
}
