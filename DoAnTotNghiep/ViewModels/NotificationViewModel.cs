using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class NotificationViewModel
    {
        public NotificationViewModel()
        {

        }
        public NotificationViewModel(Notification model, string host)
        {
            this.ImageUrl = host + model.ImageUrl;
            this.Title = model.Title;
            this.Content = model.Content;
            this.CreatedDate = model.CreatedDate;
            this.IsSeen = model.IsSeen;
            this.IdType = model.IdType;
            this.Type = model.Type;
            this.Id = model.Id;
        }

        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int Type { get; set; } = 0;
        public bool IsSeen { get; set; } = false;
        public int IdType { get; set; } = 0;
        public int Id { get; set; } = 0;
    }
}
