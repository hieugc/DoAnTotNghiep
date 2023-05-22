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

        public Notification WaitingRequestNotification(House house, Request request, User user)
        {
            return new Notification()
            {
                Title = NotificationType.RequestTitle,
                Content = "Bạn vừa tạo yêu cầu đổi nhà "
                                            + house.Name
                                            + " của "
                                            + user.FirstName
                                            + " "
                                            + user.LastName,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = house.Users.Id,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert
            };
        }
        public Notification RejectRequestNotification(House house, Request request, int idSend)
        {
            return new Notification()
            {
                Title = NotificationType.RequestTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = idSend,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert,
                Content = "Yêu cầu của bạn đến nhà "
                                        + house.Name
                                        + " bị từ chối"
            };
        }
        public Notification AcceptRequestNotification(House house, Request request, int idSend)
        {
            return new Notification()
            {
                Title = NotificationType.RequestTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = idSend,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert,
                Content = "Yêu cầu của bạn đến nhà "
                                        + house.Name
                                        + " đã được chấp nhận"
            };
        }

        public Notification CheckInRequestNotification(House house, Request request, int idSend)
        {
            return new Notification()
            {
                Title = NotificationType.RequestTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = idSend,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert,
                Content = "Bạn đã check-in "
                                + house.Name
                                + " hệ thống đã gửi thông tin đến email của bạn"
            };
        }

        public Notification CheckOutRequestNotification(House house, Request request, int idSend)
        {
            return new Notification()
            {
                Title = NotificationType.RequestTitle,
                CreatedDate = DateTime.Now,
                Type = NotificationType.REQUEST,
                IdUser = idSend,
                IdType = request.Id,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert,
                Content = "Bạn đã check-out "
                            + house.Name
                            + " hãy viết cảm nhận của mình sau chuyển đi và nhận thưởng từ hệ thống"
            };
        }

        public Notification FeedBackNotification(Request request, int idSend)
        {
            return new Notification()
            {
                IdType = request.Id,
                IdUser = idSend,
                Title = NotificationType.RatingTitle,
                Content = "Bạn có đánh giá mới",
                CreatedDate = DateTime.Now,
                IsSeen = false,
                ImageUrl = NotificationImage.Alert,
                Type = NotificationType.RATING
            };
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
